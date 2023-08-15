using UnityEngine;
using System.Collections;

// TODO: Get rid of mono behaviour
public class PlayerController : MonoBehaviour
{
	/*
	stores simulated player state, updated by PlayerSimulator
	calculates positions between ticks as updates to the Transform
	*/

	public float acc = 4f;
    public float gravity = -9.8f;

	private Vector3 velocity = Vector3.zero;

	// update every tick based on player input
    public Transform _transform;

	
	private float momentumFac = 0f;
	// private bool isAppliedJump = false;

	// forward movement? 
	public float Move { get; set; }
    public bool Jump { get; set; }
    public bool IsGrounded { get; set; }

    public void Tick(float deltaTime)
	{
		/*
		Considers the current controller state and updates the transform position
		to that at the end of deltaTime

		NOTE: updates the velocity & transform
		TODO: account for accel, move newVelocity up
		*/
		Vector3 displacement = Vector3.zero;

		if (Move > 0f)
		{
			velocity.x = Move; // assume instant horizontal accel
        }

		if (IsGrounded)
		{
			velocity.y = 0f;
			return transform.Translate(velocity * deltaTime);
		}

		// Player in air for anything below, have to account for velocity diffs
		// NOTE: ignoring this.appliedJump
		if (Jump) {
			velocity.y = 10f; // assume instant vertical accel too
		}

		Vector3 newVelocity = velocity;
		newVelocity.y -= gravity * deltaTime; // velocity at end of jump
		
		Vector3 displacement = (velocity + newVelocity) * deltaTime / 2f;

		transform.Translate(displacement);
		this.velocity = newVelocity
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

