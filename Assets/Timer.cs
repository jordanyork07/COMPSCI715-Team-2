using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    float remainingTime = 15;

    public static List<string> sceneHistory = new List<string>();

    void Update()
    {
        sceneHistory.Add(SceneManager.GetActiveScene().name);

        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            if (remainingTime < 11)
            {
                timerText.color = Color.red;
            }
        }
        else if (remainingTime < 0)
        {
            remainingTime = 0;
            try
            {
                SceneManager.LoadScene("timesupscreen");
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error loading scene: " + e.Message);
            }
        }
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
