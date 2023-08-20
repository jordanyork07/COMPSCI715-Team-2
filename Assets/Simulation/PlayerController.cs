using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.Windows;
using static PlayerController;

public class PlayerController
{
    public struct InputState
    {
        public Vector2 move;
        public Vector2 look;
        public bool jump;
        public bool sprint;
        public bool analogMovement;

        public bool forSimulationSignalGrounded;
    }

    [Header("Player")]
    [Tooltip("Move speed of the character in m/s")]
    //public float MoveSpeed = 2.0f;
    public float MoveSpeed = 5.335f;

    [Tooltip("Sprint speed of the character in m/s")]
    public float SprintSpeed = 5.335f;

    [Tooltip("How fast the character turns to face movement direction")]
    [Range(0.0f, 0.3f)]
    public float RotationSmoothTime = 0.12f;

    [Tooltip("Acceleration and deceleration")]
    public float SpeedChangeRate = 10.0f;

    public AudioClip LandingAudioClip;
    public AudioClip[] FootstepAudioClips;
    [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

    [Space(10)]
    [Tooltip("The height the player can jump")]
    public float JumpHeight = 2.0f;

    [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
    //public float Gravity = -15.0f;
    public float Gravity = -9.81f;

    [Space(10)]
    [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
    public float JumpTimeout = 0f;

    [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
    public float FallTimeout = 0.15f;

    [Header("Player Grounded")]
    [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
    public bool Grounded = true;

    [Tooltip("Useful for rough ground")]
    public float GroundedOffset = -0.14f;

    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    public float GroundedRadius = 0.28f;

    private Vector3 velocity = Vector3.zero;

    public float StepResolution = 0.2f;
    public float JumpForce = 10f;
    public float MaxJumpTime = 0.5f; // Maximum time the jump button can be held

    // player
    private float _speed;
    private float _animationBlend;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;

    // timeout deltatime
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;

    public delegate GameObject MainCameraDelegate();
    public MainCameraDelegate MainCamera;

    public delegate Animator AnimatorDelegate();
    public AnimatorDelegate Animator;

    public delegate InputState InputDelegate();
    public InputDelegate Input;

    public delegate CharacterController ControllerDelegate();
    public ControllerDelegate Controller;

    public delegate Transform TransformDelegate();
    public TransformDelegate Transform;

    public bool IsSimulation { get; set; }

    private LayerMask GroundLayers;

    // host-specific variables (TRY REFACTOR!)
    private bool _hasAnimator;

    // animation IDs
    private int _animIDSpeed;
    private int _animIDGrounded;
    private int _animIDJump;
    private int _animIDFreeFall;
    private int _animIDMotionSpeed;

    public PlayerController(LayerMask groundLayers, TransformDelegate transform, MainCameraDelegate mainCamera, AnimatorDelegate animator, InputDelegate input, ControllerDelegate controller)
    {
        MainCamera = mainCamera;
        Animator = animator;
        _hasAnimator = animator() != null;
        Input = input;
        Controller = controller;
        GroundLayers = groundLayers;
        Transform = transform;
    }

    public void Start()
    {
        // reset our timeouts on start
        _jumpTimeoutDelta = JumpTimeout;
        _fallTimeoutDelta = FallTimeout;

        AssignAnimationIDs();
    }

    private void AssignAnimationIDs()
    {
        _animIDSpeed = UnityEngine.Animator.StringToHash("Speed");
        _animIDGrounded = UnityEngine.Animator.StringToHash("Grounded");
        _animIDJump = UnityEngine.Animator.StringToHash("Jump");
        _animIDFreeFall = UnityEngine.Animator.StringToHash("FreeFall");
        _animIDMotionSpeed = UnityEngine.Animator.StringToHash("MotionSpeed");
    }

    private void InternalTick(float currentTime, float deltaTime)
    {
        JumpAndGravity(deltaTime);
        GroundedCheck();
        Move(deltaTime);

        Transform().position += velocity * deltaTime;
    }

    public delegate void PushPathVisualiserNode(Vector3 position);

    public void Tick(float currentTime, float deltaTime, PushPathVisualiserNode visDelegate)
    {
        Tick(currentTime, deltaTime, visDelegate, false);
    }

    public void Tick(float currentTime, float deltaTime, PushPathVisualiserNode visDelegate, bool step)
    {
        if (step)
        {
            float i;
            for (i = 0; i + StepResolution < deltaTime; i += StepResolution)
            {
                // Debug.Log("dt: " + StepResolution);
                currentTime += StepResolution;
                InternalTick(currentTime, StepResolution);
                visDelegate(Transform().position);
            }

            var remainder = deltaTime - i;
            if (remainder > 0)
            {
                // Debug.Log("dt(r): " + remainder);
                InternalTick(currentTime, remainder);
                visDelegate(Transform().position);    
            }
        }
        else
        {
            InternalTick(currentTime, deltaTime);
            visDelegate(Transform().position);
        }
        
    }

    private void Move(float deltaTime)
    {
        // set target speed based on move speed, sprint speed and if sprint is pressed
        float targetSpeed = Input().sprint ? SprintSpeed : MoveSpeed;

        // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

        // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is no input, set the target speed to 0
        if (Input().move == Vector2.zero) targetSpeed = 0.0f;

        // a reference to the players current horizontal velocity
        float currentHorizontalSpeed = new Vector3(Controller().velocity.x, 0.0f, Controller().velocity.z).magnitude;

        float speedOffset = 0.1f;
        float inputMagnitude = Input().analogMovement ? Input().move.magnitude : 1f;

        // accelerate or decelerate to target speed
        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            // creates curved result rather than a linear one giving a more organic speed change
            // note T in Lerp is clamped, so we don't need to clamp our speed
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                deltaTime * SpeedChangeRate);

            // round speed to 3 decimal places
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        // Debug.Log("Speed: " + _speed);
        // Debug.Log("Delta Time: " + deltaTime);

        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, deltaTime * SpeedChangeRate);
        if (_animationBlend < 0.01f) _animationBlend = 0f;

        // normalise input direction
        Vector3 inputDirection = new Vector3(Input().move.x, 0.0f, Input().move.y).normalized;

        // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is a move input rotate player when the player is moving
        if (Input().move != Vector2.zero)
        {
            if (MainCamera() != null)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  MainCamera().transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(Transform().eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    RotationSmoothTime);

                var oldRot = Transform().rotation;

                // rotate to face input direction relative to camera position
                Transform().rotation = Quaternion.Euler(0.0f, rotation, 0.0f);

                if (Transform().rotation != oldRot)
                {
                    Debug.Log("eeeee");
                }
            }
            else
            {
                Debug.Log("Main Camera is null - not rotating!");
            }
        }


        Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

        // move the player
        Controller().Move(targetDirection.normalized * (_speed * deltaTime) +
                            new Vector3(0.0f, _verticalVelocity, 0.0f) * deltaTime);

        // update animator if using character
        if (_hasAnimator)
        {
            Animator().SetFloat(_animIDSpeed, _animationBlend);
            Animator().SetFloat(_animIDMotionSpeed, inputMagnitude);
        }
    }

    private void GroundedCheck()
    {
        if (IsSimulation && Input().forSimulationSignalGrounded)
        {
            Grounded = true;
        }
        else
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(Transform().position.x, Transform().position.y - GroundedOffset,
                Transform().position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);   
        }

        // update animator if using character
        if (_hasAnimator)
        {
            Animator().SetBool(_animIDGrounded, Grounded);
        }
    }

    private void JumpAndGravity(float deltaTime)
    {
        if (Grounded)
        {
            // reset the fall timeout timer
            _fallTimeoutDelta = FallTimeout;

            // update animator if using character
            if (_hasAnimator)
            {
                Animator().SetBool(_animIDJump, false);
                Animator().SetBool(_animIDFreeFall, false);
            }

            // stop our velocity dropping infinitely when grounded
            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }

            // Jump
            if (Input().jump && _jumpTimeoutDelta <= 0.0f)
            {
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                // update animator if using character
                if (_hasAnimator)
                {
                    Animator().SetBool(_animIDJump, true);
                }
                
                if (IsSimulation)
                {
                    Grounded = false; // Awful
                }
            }

            // jump timeout
            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= deltaTime;
            }
        }
        else
        {
            // reset the jump timeout timer
            _jumpTimeoutDelta = JumpTimeout;

            // fall timeout
            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= deltaTime;
            }
            else
            {
                // update animator if using character
                if (_hasAnimator)
                {
                    Animator().SetBool(_animIDFreeFall, true);
                }
            }

            // if we are not grounded, do not jump
            var input = Input();
            input.jump = false;
        }

        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += Gravity * deltaTime;
        }

        if (IsSimulation && Grounded)
        {
            _verticalVelocity = 0;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (Grounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Gizmos.DrawSphere(
            new Vector3(Transform().position.x, Transform().position.y - GroundedOffset, Transform().position.z),
            GroundedRadius);
    }
}
