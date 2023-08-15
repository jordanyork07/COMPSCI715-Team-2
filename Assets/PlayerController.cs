using UnityEngine;
using System.Collections;

// TODO: Get rid of mono behaviour
public class PlayerController : MonoBehaviour
{

	public float acc = 4f;
    public float gravity = -9.8f;

	private Vector3 velocity = Vector3.zero;

    public Transform _transform;
	private float momentumFac = 0f;
	private bool isAppliedJump = false;

	public float Move { get; set; }
    public bool Jump { get; set; }
    public bool IsGrounded { get; set; }

    public void Tick(float deltaTime)
	{
		if (Move > 0)
		{
			velocity.x = Move;
        }

		if (Jump)
		{
			velocity.y = Move;
		}

		velocity.y -= gravity * deltaTime;
		transform.Translate(velocity * deltaTime);
	}

	public struct PossibilityArc
	{
		Vector3 startPoint;
	}

    public PossibilityArc SimulateJumpArc()
	{
		return new PossibilityArc();
	}
}

