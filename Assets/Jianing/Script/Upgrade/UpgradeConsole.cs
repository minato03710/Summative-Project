using UnityEngine;

public class UpgradeConsole : MonoBehaviour
{
    [Header("Interaction")]
    public float interactionRange = 3f;

    [Header("Interaction Text")]
    public GameObject interactionText;

    [Header("Upgrade UI")]
    public GameObject upgradeUI;

    private Transform player;
    private bool playerInRange;

    private bool ownsPause;
    private float previousTimeScale = 1f;

    private void Start()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
            player = playerObject.transform;

        if (upgradeUI != null)
            upgradeUI.SetActive(false);

        if (interactionText != null)
            interactionText.SetActive(false);
    }

    private void Update()
    {
        if (ownsPause &&
            (upgradeUI == null || !upgradeUI.activeInHierarchy))
        {
            CloseUpgradeUI();
        }

        // Prefer the player that survives scene changes.
        if (PlayerPersistence.Instance != null)
        {
            player = PlayerPersistence.Instance.transform;
        }
        else if (player == null)
        {
            GameObject playerObject =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
                player = playerObject.transform;
        }

        if (player == null)
            return;

        playerInRange =
            Vector3.Distance(transform.position, player.position)
                <= interactionRange;

        bool uiOpen =
            upgradeUI != null && upgradeUI.activeSelf;

        PlayerInputReader input = PlayerInputReader.Instance;

        bool canInteract =
            input != null &&
            input.GameplayAllowed &&
            playerInRange &&
            !uiOpen;

        if (interactionText != null)
            interactionText.SetActive(canInteract);

        if (canInteract && input.InteractPressed)
            OpenUpgradeUI();
    }

    public void OpenUpgradeUI()
    {
        if (upgradeUI == null ||
            upgradeUI.activeSelf ||
            Time.timeScale <= 0f)
        {
            return;
        }

        previousTimeScale = Time.timeScale;
        ownsPause = true;

        GamepadMenu menu =
            upgradeUI.GetComponent<GamepadMenu>();

        if (menu == null)
            menu = upgradeUI.AddComponent<GamepadMenu>();

        menu.onCancel.RemoveListener(CloseUpgradeUI);
        menu.onCancel.AddListener(CloseUpgradeUI);

        upgradeUI.SetActive(true);
        Time.timeScale = 0f;

        if (interactionText != null)
            interactionText.SetActive(false);
    }

    public void CloseUpgradeUI()
    {
        if (upgradeUI != null)
            upgradeUI.SetActive(false);

        if (ownsPause)
        {
            ownsPause = false;
            Time.timeScale = previousTimeScale;

            if (PlayerInputReader.Instance != null)
                PlayerInputReader.Instance.SuppressGameplay();
        }

        playerInRange =
            player != null &&
            Vector3.Distance(transform.position, player.position)
                <= interactionRange;

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
        if (ownsPause)
            CloseUpgradeUI();

        if (interactionText != null)
            interactionText.SetActive(false);
    }
}