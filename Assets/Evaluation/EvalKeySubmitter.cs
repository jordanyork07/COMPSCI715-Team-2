using System;
using DefaultNamespace;
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
                SceneLoader.BruteForceSceneLoad("tutorial-ready");
                Evaluator.Key.Print();
            }
            catch (Exception e)
            {
                error.gameObject.SetActive(true);
                Debug.LogException(e);
            }
        }
    }
}