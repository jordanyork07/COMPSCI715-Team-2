using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(PlayerSimulator))]
    public class PlayerSimulatorEditor : UnityEditor.Editor
    {
        private PlayerSimulator _playerSimulator;

        private void OnEnable()
        {
            _playerSimulator = (PlayerSimulator)target;
        }


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Generate"))
            {
                var actions = _playerSimulator.GeneratePath();
                _playerSimulator.SimulateActionList(actions);
            }
        }
    }
}