using System;
using UnityEngine;

namespace Evaluation
{
    public class CursorLock : MonoBehaviour
    {
        public void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}