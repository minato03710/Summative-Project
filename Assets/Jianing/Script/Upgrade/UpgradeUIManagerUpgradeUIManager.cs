using UnityEngine;
using TMPro;

public class UpgradeUIManager : MonoBehaviour
{
// =========================================================
// UI Text
// =========================================================


[Header("UI Text")]

    public TMP_Text goldText;

    public TMP_Text attackText;
    public TMP_Text healthText;

    public TMP_Text moveSpeedText;
    public TMP_Text jumpHeightText;

    public TMP_Text dashSpeedText;
    public TMP_Text dashCooldownText;

    public TMP_Text attackCooldownText;
    public TMP_Text attackRangeText;

    public TMP_Text companionAttackText;


    // =========================================================
    // Player
    // =========================================================

    [Header("Player")]

    public PlayerUpgradeManager playerUpgradeManager;

    public PlayerAttack playerAttack;
    public PlayerHealth playerHealth;
    public PlayerMovement playerMovement;
    public PlayerDash playerDash;

    public ResourceManager resourceManager;


    // =========================================================
    // Companion
    // =========================================================

    [Header("Companion")]

    public CompanionUpgrade companionUpgrade;
    public CompanionCombat companionCombat;


    // =========================================================
    // Start
    // =========================================================

    void Start()
    {
        FindPlayerComponents();

        FindCompanionComponents();

        UpdateUI();
    }


    // =========================================================
    // Find Player
    // =========================================================

    void FindPlayerComponents()
    {
        GameObject player =
            GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogWarning(
                "UpgradeUIManager: ’“≤ªµΩ Player!"
            );

            return;
        }


        if (playerUpgradeManager == null)
        {
            playerUpgradeManager =
                player.GetComponent<PlayerUpgradeManager>();
        }


        if (playerAttack == null)
        {
            playerAttack =
                player.GetComponent<PlayerAttack>();
        }


        if (playerHealth == null)
        {
            playerHealth =
                player.GetComponent<PlayerHealth>();
        }


        if (playerMovement == null)
        {
            playerMovement =
                player.GetComponent<PlayerMovement>();
        }


        if (playerDash == null)
        {
            playerDash =
                player.GetComponent<PlayerDash>();
        }


        if (resourceManager == null)
        {
            resourceManager =
                player.GetComponent<ResourceManager>();
        }
    }


    // =========================================================
    // Find Companion
    // =========================================================

    void FindCompanionComponents()
    {
        if (companionUpgrade == null)
        {
            companionUpgrade =
                FindFirstObjectByType<CompanionUpgrade>();
        }


        if (companionCombat == null)
        {
            companionCombat =
                FindFirstObjectByType<CompanionCombat>();
        }
    }


    // =========================================================
    // Update
    // =========================================================

    void Update()
    {
        if (!gameObject.activeSelf)
            return;

        UpdateUI();
    }


    // =========================================================
    // Player Attack
    // =========================================================

    public void UpgradeAttack()
    {
        if (playerUpgradeManager == null)
            return;

        bool success =
            playerUpgradeManager.UpgradeAttack();

        if (success)
        {
            UpdateUI();
        }
    }


    // =========================================================
    // Player Health
    // =========================================================

    public void UpgradeHealth()
    {
        if (playerUpgradeManager == null)
            return;

        bool success =
            playerUpgradeManager.UpgradeHealth();

        if (success)
        {
            UpdateUI();
        }
    }


    // =========================================================
    // Move Speed
    // =========================================================

    public void UpgradeMoveSpeed()
    {
        if (playerUpgradeManager == null)
            return;

        bool success =
            playerUpgradeManager.UpgradeMoveSpeed();

        if (success)
        {
            UpdateUI();
        }
    }


    // =========================================================
    // Jump Height
    // =========================================================

    public void UpgradeJumpHeight()
    {
        if (playerUpgradeManager == null)
            return;

        bool success =
            playerUpgradeManager.UpgradeJumpHeight();

        if (success)
        {
            UpdateUI();
        }
    }


    // =========================================================
    // Dash Speed
    // =========================================================

    public void UpgradeDashSpeed()
    {
        if (playerUpgradeManager == null)
            return;

        bool success =
            playerUpgradeManager.UpgradeDashSpeed();

        if (success)
        {
            UpdateUI();
        }
    }


    // =========================================================
    // Dash Cooldown
    // =========================================================

    public void UpgradeDashCooldown()
    {
        if (playerUpgradeManager == null)
            return;

        bool success =
            playerUpgradeManager.UpgradeDashCooldown();

        if (success)
        {
            UpdateUI();
        }
    }


    // =========================================================
    // Attack Cooldown
    // =========================================================

    public void UpgradeAttackCooldown()
    {
        if (playerUpgradeManager == null)
            return;

        bool success =
            playerUpgradeManager.UpgradeAttackCooldown();

        if (success)
        {
            UpdateUI();
        }
    }


    // =========================================================
    // Attack Range
    // =========================================================

    public void UpgradeAttackRange()
    {
        if (playerUpgradeManager == null)
            return;

        bool success =
            playerUpgradeManager.UpgradeAttackRange();

        if (success)
        {
            UpdateUI();
        }
    }


    // =========================================================
    // Companion Attack
    // =========================================================

    public void UpgradeCompanionAttack()
    {
        if (companionUpgrade == null)
            return;

        bool success =
            companionUpgrade.UpgradeAttack(
                resourceManager
            );

        if (success)
        {
            UpdateUI();
        }
    }


    // =========================================================
    // Update UI
    // =========================================================

    void UpdateUI()
    {
        // -------------------------
        // Gold
        // -------------------------

        if (resourceManager != null &&
            goldText != null)
        {
            goldText.text =
                "Gold: " +
                resourceManager.gold;
        }


        // -------------------------
        // Attack
        // -------------------------

        if (playerAttack != null &&
            attackText != null)
        {
            attackText.text =
                "Attack: " +
                playerAttack.attackDamage +
                "\nCost: " +
                playerUpgradeManager.attackUpgradeCost;
        }


        // -------------------------
        // Health
        // -------------------------

        if (playerHealth != null &&
            healthText != null)
        {
            healthText.text =
                "Health: " +
                playerHealth.maxHealth +
                "\nCost: " +
                playerUpgradeManager.healthUpgradeCost;
        }


        // -------------------------
        // Move Speed
        // -------------------------

        if (playerMovement != null &&
            moveSpeedText != null)
        {
            moveSpeedText.text =
                "Move Speed: " +
                playerMovement.moveSpeed +
                "\nCost: " +
                playerUpgradeManager.moveSpeedUpgradeCost;
        }


        // -------------------------
        // Jump Height
        // -------------------------

        if (playerMovement != null &&
            jumpHeightText != null)
        {
            jumpHeightText.text =
                "Jump Height: " +
                playerMovement.jumpHeight +
                "\nCost: " +
                playerUpgradeManager.jumpHeightUpgradeCost;
        }


        // -------------------------
        // Dash Speed
        // -------------------------

        if (playerDash != null &&
            dashSpeedText != null)
        {
            dashSpeedText.text =
                "Dash Speed: " +
                playerDash.dashSpeed +
                "\nCost: " +
                playerUpgradeManager.dashSpeedUpgradeCost;
        }


        // -------------------------
        // Dash Cooldown
        // -------------------------

        if (playerDash != null &&
            dashCooldownText != null)
        {
            dashCooldownText.text =
                "Dash Cooldown: " +
                playerDash.dashCooldown.ToString("F2") +
                "\nCost: " +
                playerUpgradeManager.dashCooldownUpgradeCost;
        }


        // -------------------------
        // Attack Cooldown
        // -------------------------

        if (playerAttack != null &&
            attackCooldownText != null)
        {
            attackCooldownText.text =
                "Attack Cooldown: " +
                playerAttack.attackCooldown.ToString("F2") +
                "\nCost: " +
                playerUpgradeManager.attackCooldownUpgradeCost;
        }


        // -------------------------
        // Attack Range
        // -------------------------

        if (playerAttack != null &&
            attackRangeText != null)
        {
            attackRangeText.text =
                "Attack Range: " +
                playerAttack.attackRange.ToString("F1") +
                "\nCost: " +
                playerUpgradeManager.attackRangeUpgradeCost;
        }


        // -------------------------
        // Companion Attack
        // -------------------------

        if (companionCombat != null &&
            companionAttackText != null)
        {
            companionAttackText.text =
                "Companion Attack: " +
                companionCombat.attackDamage +
                "\nCost: " +
                companionUpgrade.attackUpgradeCost;
        }
    }


    // =========================================================
    // Close UI
    // =========================================================

    public void CloseUI()
    {
        gameObject.SetActive(false);
    }


}
