using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelNumberUI : MonoBehaviour
{
    public Text text;

    private void Start()
    {
        text.text = SceneManager.GetActiveScene().name;
    }
}