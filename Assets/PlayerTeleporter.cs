using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerTeleporter : MonoBehaviour
{

    [System.Serializable]
    public class OnCollisionEvent : UnityEvent {}

    public OnCollisionEvent OnCollision;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        OnCollision.Invoke();
        Cursor.lockState = CursorLockMode.None;
    }

    // Update is called once per frame
    void Update()
    {
           
    }
}
