using System.Text;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;


namespace Evaluation
{

    
    public class EvalKeyGeneratorObject : MonoBehaviour
    {   
        public List<string> keys = new List<string>();
        public void Generate()
        {
            keys.Clear();
            string _keys =  EvalKeyGenerator.Generate();
            // split the string into an array of strings
            string[] _keysArray = _keys.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            // add each string to the list
            keys.AddRange(_keysArray);

        }
    }
}