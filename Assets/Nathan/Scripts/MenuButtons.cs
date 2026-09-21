using UnityEngine;
using UnityEngine.SceneManagement;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public string PlayGame;
    public GameObject MainMenu;
    public GameObject SettingsMenu;
    public GameObject AudioMenu;
    public GameObject ControlsMenu;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    //MAIN MENU BUTTONS

    public void StartGameScene()
    {
        SceneManager.LoadScene(PlayGame);
    }

    public void CloseGame()
    {
        Application.Quit();
    }


    // MENU SWAPPING SCREENS //

    public void SettingsMenuScreen()
    {
        MainMenu.SetActive(false);
        SettingsMenu.SetActive(true);
    }


    // RETURN/BACK BUTTON SCREENS //

   
}
