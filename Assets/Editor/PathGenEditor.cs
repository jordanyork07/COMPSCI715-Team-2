using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(PathGen))]
    public class PathGenEditor : UnityEditor.Editor
    {
        private PathGen pathGen;

        private void OnEnable()
        {
            pathGen = (PathGen)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Generate"))
            {
                pathGen.MarkDirty();
            }
        }
    }
}