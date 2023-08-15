using UnityEngine;

public class PlayerController
{
    public Vector3 Position { get; private set; }

    public float Move { get; set; }
    public bool Jump { get; set; }

    private Vector3 velocity = Vector3.zero;

    public float StepResolution = 0.2f;
    public float JumpForce = 10f;
    public float MaxJumpTime = 0.5f; // Maximum time the jump button can be held
    public float Gravity = Physics.gravity.y * 0.8f;

    private float jumpStartTime;
    private bool isJumping;

    private void InternalTick(float currentTime, float deltaTime)
    {
        UpdateMovement();
        ApplyJump(currentTime);
        ApplyGravity(deltaTime);

        Position += velocity * StepResolution;
    }

    public delegate void PushPathVisualiserNode(Vector3 position);

    public void Tick(float currentTime, float deltaTime, PushPathVisualiserNode visDelegate)
    {
        float i;
        for (i = 0; i < deltaTime; i += StepResolution)
        {
            currentTime += i;
            InternalTick(currentTime, StepResolution);
            visDelegate(this.Position);
        }

        var remainder = deltaTime - i;
        InternalTick(currentTime, remainder);
        visDelegate(this.Position);
    }

    private void ApplyJump(float currentTime)
    {
        if (Jump && (!isJumping || currentTime - jumpStartTime <= MaxJumpTime))
        {
            velocity.y = JumpForce;
            jumpStartTime = currentTime;
            isJumping = true;
        }
        else
        {
            isJumping = false;
            jumpStartTime = float.MaxValue;
        }
    }

    private void UpdateMovement()
    {
        velocity.x = Move;
    }

    private void ApplyGravity(float deltaTime)
    {
        velocity.y += Gravity * deltaTime;
    }
}
