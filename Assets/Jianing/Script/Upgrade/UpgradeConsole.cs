using UnityEngine;
using UnityEngine.InputSystem;

public class UpgradeConsole : MonoBehaviour
{
    [Header("Interaction")]
    public float interactionRange = 3f;


[Header("Interaction Text")]
    public GameObject interactionText;

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

        if (interactionText != null)
        {
            interactionText.SetActive(false);
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

        // 玩家进入范围
        if (playerInRange)
        {
            if (interactionText != null &&
                !upgradeUI.activeSelf)
            {
                interactionText.SetActive(true);
            }

            if (Keyboard.current != null &&
                Keyboard.current.eKey.wasPressedThisFrame)
            {
                ToggleUpgradeUI();
            }
        }
        // 玩家离开范围
        else
        {
            if (interactionText != null)
            {
                interactionText.SetActive(false);
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

        if (newState)
        {
            // 打开升级界面
            Time.timeScale = 0f;

            if (interactionText != null)
            {
                interactionText.SetActive(false);
            }

            Debug.Log("Upgrade UI Opened");
        }
        else
        {
            // 关闭升级界面
            Time.timeScale = 1f;

            if (playerInRange &&
                interactionText != null)
            {
                interactionText.SetActive(true);
            }

            Debug.Log("Upgrade UI Closed");
        }
    }

    public void CloseUpgradeUI()
    {
        if (upgradeUI != null)
        {
            upgradeUI.SetActive(false);
        }

        Time.timeScale = 1f;

        if (playerInRange &&
            interactionText != null)
        {
            interactionText.SetActive(true);
        }

        Debug.Log("Upgrade UI Closed");
    }


}
