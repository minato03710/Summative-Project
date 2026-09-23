using UnityEngine;
using UnityEngine.InputSystem;

public class ShopInteraction : MonoBehaviour
{
    [Header("Shop UI")]
    public GameObject shopUI;


[Header("Interaction")]
    public GameObject interactionText;

    private bool playerInRange = false;

    void Start()
    {
        if (shopUI != null)
            shopUI.SetActive(false);

        if (interactionText != null)
            interactionText.SetActive(false);
    }

    void Update()
    {
        if (!playerInRange)
            return;

        if (Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            OpenShop();
        }
    }

    void OpenShop()
    {
        if (shopUI == null)
            return;

        shopUI.SetActive(true);

        Time.timeScale = 0f;

        if (interactionText != null)
            interactionText.SetActive(false);
    }

    public void CloseShop()
    {
        if (shopUI != null)
            shopUI.SetActive(false);

        Time.timeScale = 1f;

        if (playerInRange &&
            interactionText != null)
        {
            interactionText.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = true;

        if (interactionText != null)
            interactionText.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = false;

        if (interactionText != null)
            interactionText.SetActive(false);
    }


}
