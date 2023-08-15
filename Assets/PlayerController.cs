using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{

	public float acc = 4f;
    public float gravity = -9.8f;

    private readonly Transform _transform;

	public struct PossibilityArc
	{
		Vector3 startPoint;
	}

    public PossibilityArc SimulateJumpArc()
	{
		return new PossibilityArc();
	}
}

