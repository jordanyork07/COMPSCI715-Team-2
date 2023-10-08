using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimesUp : MonoBehaviour
{ 
    public void Continue()
    {
        if (Timer.sceneHistory.Count > 0)
        {
            string previousScene = Timer.sceneHistory[Timer.sceneHistory.Count - 1];

            if (previousScene.Equals("level_2"))
            {
                SceneManager.LoadScene("level_3");
            }
        }
    }
}

