using DefaultNamespace;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    public static UI instance;

    [SerializeField] private GameObject menu;


    private void Awake()
    {
        instance = this;
    }

    void Start()
    {

    }


    void Update()
    {

    }

 
    public void OpenEndScreen()
    {
        Time.timeScale = 0;
        menu.SetActive(true);
    }
    
    public void LoadScene()
    {   
        Debug.Log("LoadScene");
        // int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        // int nextSceneIndex = currentSceneIndex + 1;
        // get scene name level_1 etc and increment and load
        string currentSceneName = SceneManager.GetActiveScene().name;
        string nextScene = currentSceneName.Split('_')[0] + "_" + (int.Parse(currentSceneName.Split('_')[1]) + 1);
        Debug.Log(nextScene);
        SceneLoader.BruteForceSceneLoad(nextScene);
    }
}
