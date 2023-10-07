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
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        SceneManager.LoadScene(nextSceneIndex);
    }
}
