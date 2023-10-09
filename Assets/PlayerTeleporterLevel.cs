using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerTeleporterLevel : PlayerTeleporter
{
    public LevelManager manager;
    public GameObject player;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void PlayerFinishedLevel()
    {
        var cc = player.GetComponent<CharacterController>();
        cc.enabled = false;
        player.transform.position = new Vector3(0, 2, -4);
        player.transform.rotation = new Quaternion();
        cc.enabled = true;
        
        manager.NextSimulation();
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerFinishedLevel();
    }

    // Update is called once per frame
    void Update()
    {
           
    }
}
