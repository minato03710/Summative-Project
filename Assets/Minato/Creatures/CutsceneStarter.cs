using UnityEngine;

public class CutsceneStarter : MonoBehaviour
{

    public bool bossStart;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            bossStart = true;
        }
    }
}
