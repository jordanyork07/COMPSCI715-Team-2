using DefaultNamespace;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Evaluation
{
    public class SceneNavigator : MonoBehaviour
    {
        void Update() {
            // check if current scene is a level
            if (SceneManager.GetActiveScene().name.Contains("Level")) {
                Cursor.lockState = CursorLockMode.Locked;
                return;
            }
            Cursor.lockState = CursorLockMode.None;
        }
        public void GoToScene(string scene)
        {
            SceneLoader.BruteForceSceneLoad(scene);
            Cursor.lockState = CursorLockMode.None;
        }

        public void NextPlayScene()
        {
            Evaluator.LoadNextScene();
            Cursor.lockState = CursorLockMode.None;
        }
    }
}