using UnityEngine;

public class ShopInteraction : MonoBehaviour
{
    [Header("Shop UI")]
    public GameObject shopUI;

    [Header("Interaction")]
    public GameObject interactionText;

    [Min(0.1f)]
    public float interactionRange = 3f;

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
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
            player = playerObject.transform;
    }

    private void Update()
    {
        if (shopOpen &&
            (shopUI == null || !shopUI.activeInHierarchy))
        {
            CloseShop();
        }

        if (player == null)
            FindPlayer();

        playerInRange =
            player != null &&
            Vector3.Distance(transform.position, player.position)
                <= interactionRange;

        PlayerInputReader input = PlayerInputReader.Instance;

        bool canInteract =
            input != null &&
            input.GameplayAllowed &&
            playerInRange &&
            shopUI != null &&
            !shopOpen &&
            !shopUI.activeSelf &&
            Time.timeScale > 0f;

        if (interactionText != null)
            interactionText.SetActive(canInteract);

        if (canInteract && input.InteractPressed)
            OpenShop();
    }

    public void OpenShop()
    {
        if (!playerInRange ||
            shopUI == null ||
            shopOpen ||
            Time.timeScale <= 0f)
        {
            return;
        }

        previousTimeScale = Time.timeScale;
        shopOpen = true;

        GamepadMenu menu = shopUI.GetComponent<GamepadMenu>();

        if (menu == null)
            menu = shopUI.AddComponent<GamepadMenu>();

        menu.onCancel.RemoveListener(CloseShop);
        menu.onCancel.AddListener(CloseShop);

        shopUI.SetActive(true);
        Time.timeScale = 0f;

        if (interactionText != null)
            interactionText.SetActive(false);
    }

    public void CloseShop()
    {
        if (shopOpen)
        {
            shopOpen = false;
            Time.timeScale = previousTimeScale;

            if (PlayerInputReader.Instance != null)
                PlayerInputReader.Instance.SuppressGameplay();
        }

        if (shopUI != null)
            shopUI.SetActive(false);

        if (interactionText != null)
        {
            interactionText.SetActive(
                isActiveAndEnabled &&
                playerInRange &&
                Time.timeScale > 0f
            );
        }
    }

    private void OnDisable()
    {
        if (shopOpen)
            CloseShop();

        if (interactionText != null)
            interactionText.SetActive(false);
    }
}