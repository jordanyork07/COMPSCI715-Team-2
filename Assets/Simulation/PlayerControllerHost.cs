using System;
using StarterAssets;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using static PlayerController;
#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

[RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM 
[RequireComponent(typeof(PlayerInput))]
#endif
public class PlayerControllerHost : MonoBehaviour
{
    public PushPathVisualiserNode visDelegate;

    [Header("Cinemachine")]
    [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    public GameObject CinemachineCameraTarget;

    [Tooltip("For locking the camera position on all axis")]
    public bool LockCameraPosition = false;

    // cinemachine
    public float _cinemachineTargetYaw;
    public float _cinemachineTargetPitch;

    [Tooltip("How far in degrees can you move the camera up")]
    public float TopClamp = 70.0f;

    [Tooltip("How far in degrees can you move the camera down")]
    public float BottomClamp = -30.0f;

    [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
    public float CameraAngleOverride = 15.0f;

    [Tooltip("What layers the character uses as ground")]
    public LayerMask GroundLayers;


#if ENABLE_INPUT_SYSTEM
    private PlayerInput _playerInput;
#endif
    private PlayerController _delegate;
    private GameObject _mainCamera;
    private StarterAssetsInputs _input;
    private CharacterController _controller;

    private const float _threshold = 0.01f;

    

    private bool IsCurrentDeviceMouse
    {
        get
        {
#if ENABLE_INPUT_SYSTEM
            return _playerInput.currentControlScheme == "KeyboardMouse";
#else
			return false;
#endif
        }
    }


    private void Awake()
    {
        // get a reference to our main camera
        if (_mainCamera == null)
        {
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }
    }

    private void Start()
    {
        _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

        TryGetComponent(out Animator _animator);
        _input = GetComponent<StarterAssetsInputs>();
        _controller = GetComponent<CharacterController>();
#if ENABLE_INPUT_SYSTEM 
        _playerInput = GetComponent<PlayerInput>();
#else
		Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

        _delegate = new PlayerController(GroundLayers, () => transform, () => _mainCamera, () => _animator, () => _input.GetInputState(), () => _controller);
        _delegate.Start();
    }

    private void Update()
    {
        //_hasAnimator = TryGetComponent(out _animator);

        var fixedTime = Time.fixedTime;
        var deltaTime = Time.deltaTime;
        if (_delegate == null)
            Start();

         _delegate.Tick(fixedTime, deltaTime, (n) => { }, true);
    }

    private void LateUpdate()
    {
        CameraRotation();
    }

    private void CameraRotation()
    {
        var look = _input.GetInputState().look;
        // if there is an input and camera position is not fixed
        if (look.sqrMagnitude >= _threshold && !LockCameraPosition)
        {
            //Don't multiply mouse input by Time.deltaTime;
            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

            _cinemachineTargetYaw += look.x * deltaTimeMultiplier;
            _cinemachineTargetPitch += look.y * deltaTimeMultiplier;
        }   

        // clamp our rotations so our values are limited 360 degrees
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

        // Cinemachine will follow this target
        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
            _cinemachineTargetYaw, 0.0f);
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            //if (FootstepAudioClips.Length > 0)
            //{
            //    var index = Random.Range(0, FootstepAudioClips.Length);
            //    //AudioSource.PlayClipAtPoint(FootstepAudioClips[index], _transform.TransformPoint(Controller().center), FootstepAudioVolume);
            //}
        }
    }

    private void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            //AudioSource.PlayClipAtPoint(LandingAudioClip, _transform.TransformPoint(Controller().center), FootstepAudioVolume);
        }
    }
}