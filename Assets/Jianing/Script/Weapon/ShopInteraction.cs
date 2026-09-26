using UnityEngine;
using UnityEngine.InputSystem;

public class ShopInteraction : MonoBehaviour
{
    [Header("Shop UI")]
    public GameObject shopUI;

    [Header("Interaction")]
    public GameObject interactionText;
    [Min(0.1f)] public float interactionRange = 3f;

    private Transform player;
    private bool playerInRange;
    private bool shopOpen;
    private float previousTimeScale = 1f;

    private void Start()
    {
        FindPlayer();
        if (shopUI != null)
            shopUI.SetActive(false);
        if (interactionText != null)
            interactionText.SetActive(false);
    }

    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
            player = playerObject.transform;
    }

    private void Update()
    {
        // Also restore movement if another UI callback hides the panel.
        if (shopOpen && (shopUI == null || !shopUI.activeInHierarchy))
            CloseShop();

        if (player == null)
            FindPlayer();

        playerInRange = player != null &&
            Vector3.Distance(transform.position, player.position) <= interactionRange;

        bool canInteract = playerInRange && shopUI != null &&
            !shopOpen && !shopUI.activeSelf && Time.timeScale > 0f;

        if (interactionText != null)
            interactionText.SetActive(canInteract);

        // E opens the shop only. Use the close button to leave it.
        if (canInteract && Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            OpenShop();
        }
    }

    public void OpenShop()
    {
        if (!playerInRange || shopUI == null || shopOpen || Time.timeScale <= 0f)
            return;

        previousTimeScale = Time.timeScale;
        shopOpen = true;
        shopUI.SetActive(true);
        Time.timeScale = 0f;

        if (interactionText != null)
            interactionText.SetActive(false);
    }

    public void CloseShop()
    {
        // Only release a pause owned by this shop.
        if (shopOpen)
        {
            shopOpen = false;
            Time.timeScale = previousTimeScale;
        }

        if (shopUI != null)
            shopUI.SetActive(false);

        if (interactionText != null)
            interactionText.SetActive(isActiveAndEnabled && playerInRange && Time.timeScale > 0f);
    }

    private void OnDisable()
    {
        if (shopOpen)
            CloseShop();
        if (interactionText != null)
            interactionText.SetActive(false);
    }
}
