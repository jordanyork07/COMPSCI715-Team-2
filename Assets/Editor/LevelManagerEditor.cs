using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(LevelManager))]
    public class LevelManagerEditor : UnityEditor.Editor
    {
        private LevelManager _manager;

        private void OnEnable()
        {
            _manager = (LevelManager)target;
        }


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Restart"))
            {
                
                _manager.Restart();
            }

            if (GUILayout.Button("Simulate Levels"))
            {
                Debug.Log("Simulating Levels");
                _manager.SimulateLevels();
            }

            if (GUILayout.Button("Next"))
            {
                Debug.Log("Switching Level");
                _manager.NextSimulation();
            }
        }
    }
}