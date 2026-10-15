using UnityEngine;
using UnityEngine.SceneManagement;

public class Tutorial_Death : MonoBehaviour
{

    public string BadEnding2_Name;
    public PlayerHealth p_health;


    void Update()
    {
        if (p_health.currentHealth <= 0)
        {
            Debug.Log("PLAYER DEAD LOAD BAD ENDING");
            badEnding2();
        }
    }

    public void badEnding2()
    {
        SceneManager.LoadScene(BadEnding2_Name);
        Debug.Log("Bad ending loaded");
    }




}
