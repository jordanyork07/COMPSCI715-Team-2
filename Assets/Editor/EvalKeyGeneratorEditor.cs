using UnityEditor;
using UnityEngine;
using Evaluation;

namespace Editor
{
    [CustomEditor(typeof(EvalKeyGeneratorObject))]
    public class EvalKeyGeneratorEditor : UnityEditor.Editor
    {
        private EvalKeyGeneratorObject _component;
        
        private void OnEnable()
        {
            _component = (EvalKeyGeneratorObject)target;
        }


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            

            if (GUILayout.Button("Generate"))
            {
               _component.Generate();
            }

        }
    }
}