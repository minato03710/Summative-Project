using UnityEngine;
using UnityEngine.InputSystem;

public class UpgradeConsole : MonoBehaviour
{
    [Header("Interaction")]
    public float interactionRange = 3f;


[Header("Upgrade UI")]
    public GameObject upgradeUI;

    private Transform player;
    private bool playerInRange = false;

    void Start()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        if (upgradeUI != null)
        {
            upgradeUI.SetActive(false);
        }
    }

    void Update()
    {
        if (player == null)
            return;

        float distance =
            Vector3.Distance(
                transform.position,
                player.position
            );

        playerInRange =
            distance <= interactionRange;

        if (playerInRange)
        {
            if (Keyboard.current != null &&
                Keyboard.current.eKey.wasPressedThisFrame)
            {
                ToggleUpgradeUI();
            }
        }
    }

    void ToggleUpgradeUI()
    {
        if (upgradeUI == null)
            return;

        bool newState =
            !upgradeUI.activeSelf;

        upgradeUI.SetActive(newState);

        Debug.Log(
            newState
            ? "Upgrade UI Opened"
            : "Upgrade UI Closed"
        );
    }


}

