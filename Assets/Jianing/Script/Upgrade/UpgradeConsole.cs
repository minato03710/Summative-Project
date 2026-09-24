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
    private bool playerInRange;

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
        if (player == null)
            return;

        playerInRange =
            Vector3.Distance(transform.position, player.position)
            <= interactionRange;

        bool uiOpen =
            upgradeUI != null && upgradeUI.activeSelf;

        if (interactionText != null)
            interactionText.SetActive(playerInRange && !uiOpen);

        // E 只负责打开，界面打开后再次按 E 不会关闭。
        if (playerInRange && !uiOpen &&
            Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            OpenUpgradeUI();
        }
    }

    public void OpenUpgradeUI()
    {
        if (upgradeUI == null || upgradeUI.activeSelf)
            return;

        upgradeUI.SetActive(true);
        Time.timeScale = 0f;

        if (interactionText != null)
            interactionText.SetActive(false);
    }

    // 关闭按钮需要绑定这个方法。
    public void CloseUpgradeUI()
    {
        if (upgradeUI != null)
            upgradeUI.SetActive(false);

        // 恢复游戏时间，让角色可以继续移动。
        Time.timeScale = 1f;

        playerInRange =
            player != null &&
            Vector3.Distance(transform.position, player.position)
            <= interactionRange;

        if (interactionText != null)
            interactionText.SetActive(playerInRange);
    }
}