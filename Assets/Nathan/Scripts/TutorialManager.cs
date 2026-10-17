using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{

    public GameObject CompanionBefore;
    public GameObject CompanionAfter;

    public GameObject escapeBarrier;
    public GameObject escapeLights;
    public GameObject choiceTouch;

    public GameObject canvasChoiceButtons;
    public GameObject escapeCanvas;

    public string BadEnding1_Name;
    public string BadEnding2_Name;

    public GameObject EnemyGroup1;

    public bool choiceActive = false;

    //SFX
    public AudioSource BG_Music;
    public AudioSource EscapeMusic;






    private void OnTriggerEnter(Collider other)
    {
        if ((other.CompareTag("Player")) && choiceActive == false)
        {
            choiceActive = true;
            canvasChoiceButtons.SetActive(true);
        }
    }


    public void KillButton()
    {
        BadEnding1();
    }

    public void SpareButton()
    {
        canvasChoiceButtons.SetActive(false);
        escapeBarrier.SetActive(false);
        EnemyGroup1.SetActive(true);
        escapeLights.SetActive(true);
        CompanionBefore.SetActive(false);
        CompanionAfter.SetActive(true);
        escapeCanvas.SetActive(true);

        BG_Music.Stop();
        EscapeMusic.Play();
    }


    public void BadEnding1()
    {
        SceneManager.LoadScene(BadEnding1_Name);
    }

    public void BadEnding2()
    {
        SceneManager.LoadScene(BadEnding2_Name);
    }



}
