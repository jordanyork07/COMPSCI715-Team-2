using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Evaluation;

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    float remainingTime = 300;

    // Update is called once per frame
    void Update()
    {   
        var CurrentTime = DateTime.Now;
        if(remainingTime == 300){
            Evaluation.Logger.LogByEvalKey(Evaluator.Key, " Timer Started!");
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
            Evaluation.Logger.LogByEvalKey(Evaluator.Key, " Timer Ended!");

            SceneManager.LoadScene("timesupscreen");
        }
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
