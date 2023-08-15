using UnityEngine;
using System.Collections;

public class PlayerControllerHost : MonoBehaviour
{
	PlayerController playerController;

	// Use this for initialization
	void Start()
	{
		playerController.Jump = Input.GetButton("Jump");
	}

	// Update is called once per frame
	void Update()
	{
			
	}
}

