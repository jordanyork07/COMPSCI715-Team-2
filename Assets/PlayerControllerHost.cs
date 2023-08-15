using UnityEngine;
using System.Collections;

public class PlayerControllerHost : MonoBehaviour
{
	// Use this for initialization
	void Start()
	{
        var playerController = new PlayerController();
		playerController.Jump = Input.GetButton("Jump");
	}

	// Update is called once per frame
	void Update()
	{
		
	}
}

