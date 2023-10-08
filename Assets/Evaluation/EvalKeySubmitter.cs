using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Evaluation
{
    public class EvalKeySubmitter : MonoBehaviour
    {
        public GameObject error;
        public TMP_InputField input;

        public void Clicked()
        {
            error.gameObject.SetActive(false);
            
            try
            {
                Evaluator.SetEvalKey(input.text);
                SceneManager.LoadScene("mjak923_PlayerController");
            }
            catch (Exception e)
            {
                error.gameObject.SetActive(true);
                Debug.LogException(e);
            }
        }
    }
}