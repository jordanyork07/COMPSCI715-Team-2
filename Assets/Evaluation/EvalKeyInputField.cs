using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Evaluation
{
    public class EvalKeyInputField : MonoBehaviour
    {
        public GameObject error;

        public void Start()
        {
            Debug.Log("Generating Latin Squares");
            
            LatinSquareGenerator.Generate();
        }

        public void OnChangedInputField(string input)
        {
            error.gameObject.SetActive(false);
            
            try
            {
                Evaluator.SetEvalKey(input);
            }
            catch
            {
                error.gameObject.SetActive(true);
            }
        }
    }
}