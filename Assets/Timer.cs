using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    float remainingTime = 300;

    // Update is called once per frame
    void Update()
    {   
        var CurrentTime = DateTime.Now;
        if(remainingTime == 300){
            Debug.Log(CurrentTime+ " Timer Started!");
        }
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
            Debug.Log(CurrentTime+ " Time's up!");
            SceneManager.LoadScene("timesupscreen");
        }
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
