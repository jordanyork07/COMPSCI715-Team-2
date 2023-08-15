using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerSimulator : MonoBehaviour
{
	public PlayerController realController;
	public PathGen pathGen;

	// Use this for initialization
	void Start()
	{
		var actions = pathGen.GenerateRhythm(PathGen.Pattern.Regular, PathGen.Density.Medium, 20);
		SimulateActionList(actions);
	}

	// Update is called once per frame
	void Update()
	{
			
	}

	void SimulateActionList(List<PathGen.Action> actions)
	{
		var playerController = new PlayerController();
		//realController
		foreach (var action in actions)
		{
			//GetStatefulPossibilitySpace
			ApplyMove(action);
		}
	}

	void ApplyMove(PathGen.Action action)
    {

    }
}

