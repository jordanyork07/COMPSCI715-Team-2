using UnityEngine.SceneManagement;

namespace DefaultNamespace
{
    public abstract class SceneLoader
    {
        public static void BruteForceSceneLoad(string sceneName)
        {
            // https://forum.unity.com/threads/loading-scene-additively-causes-change-in-lighting.511566/
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
            // SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
        }
    }
}