using UnityEngine;
using UnityEngine.SceneManagement;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public string PlayGame;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void StartGameScene()
    {
        SceneManager.LoadScene(PlayGame);
    }

    public void CloseGame()
    {
        Application.Quit();
    }
}
