using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine.SceneManagement;

namespace Evaluation
{
    public static class Evaluator
    {
        public static EvalKey Key { get; private set; }

        private static Dictionary<int, string> _sceneMap = new()
        {
            {1, "SwingLevels"},
            {2, "RandomLevels"},
            {3, "ManualLevels"}
        };

        private static Queue<string> _queuedScenes;
        
        static Evaluator()
        {
            Key = new EvalKey();
        }

        public static void SetEvalKey(string base64)
        {
            Key = EvalKey.Decode(base64);
            Key.Print();

            _queuedScenes = new Queue<string>();
            foreach (var sceneId in Key.Order)
            {
                var sceneName = _sceneMap[sceneId];
                _queuedScenes.Enqueue(sceneName);
            }
        }

        public static void LoadNextScene()
        {
            if (_queuedScenes.TryDequeue(out var sceneName))
                SceneLoader.BruteForceSceneLoad(sceneName);
        }

        public static void LoadNextInterimScene()
        {
            if (_queuedScenes.TryPeek(out var sceneName))
                SceneLoader.BruteForceSceneLoad("timesupscreen");
            else
                SceneLoader.BruteForceSceneLoad("fin");
        }
    }
}