using UnityEngine;
using UnityEngine.SceneManagement;

namespace Evaluation
{
    public class SceneNavigator : MonoBehaviour
    {
        public void GoToScene(string scene)
        {
            SceneManager.LoadScene(scene);
            Cursor.lockState = CursorLockMode.None;
        }

        public void NextPlayScene()
        {
            Evaluator.LoadNextScene();
            Cursor.lockState = CursorLockMode.None;
        }
    }
}