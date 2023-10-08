using System.Collections;
using System.Collections.Generic;
using Evaluation;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Quit()
    {
        Application.Quit();
        Debug.Log("Player has quit the game.");
    }
}
