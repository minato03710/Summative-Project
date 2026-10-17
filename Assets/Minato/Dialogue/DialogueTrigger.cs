using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{

    [SerializeField] GameObject searchCanvas;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            searchCanvas.SetActive(true);
        }
    }
}