using UnityEngine;
using System.Collections;

// TODO: Get rid of mono behaviour
public class PlayerController : MonoBehaviour
{

	public float acc = 4f;
    public float gravity = -9.8f;

    public Transform _transform;
	private float momentumFac = 0f;
	private bool isAppliedJump = false;

	public float Move { get; set; }
    public bool Jump { get; set; }
    public bool IsGrounded { get; set; }

    public void Tick(float deltaTime)
	{

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

