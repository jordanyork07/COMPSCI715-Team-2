using System;
using UnityEngine;

namespace Evaluation
{
    public class CursorUnlock : MonoBehaviour
    {
        public void Start()
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}