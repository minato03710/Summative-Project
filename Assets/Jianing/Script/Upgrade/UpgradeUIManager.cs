using UnityEngine;
using UnityEngine.Serialization;
using TMPro;

public class UpgradeUIManager : MonoBehaviour
{
    [Header("UI Text")]
    [FormerlySerializedAs("goldText")]
    public TMP_Text resourceText;

    public TMP_Text attackText;
    public TMP_Text healthText;
    public TMP_Text moveSpeedText;
    public TMP_Text jumpHeightText;
    public TMP_Text dashSpeedText;
    public TMP_Text dashCooldownText;
    public TMP_Text attackCooldownText;
    public TMP_Text attackRangeText;
    public TMP_Text companionAttackText;

    [Header("Player")]
    public PlayerUpgradeManager playerUpgradeManager;
    public PlayerAttack playerAttack;
    public PlayerHealth playerHealth;
    public PlayerMovement playerMovement;
    public PlayerDash playerDash;
    public ResourceManager resourceManager;

    [Header("Companion")]
    public CompanionUpgrade companionUpgrade;
    public CompanionCombat companionCombat;

    private void OnEnable()
    {
        FindComponents();
        UpdateUI();
    }

    private void FindComponents()
    {
        GameObject player =
            GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            if (playerUpgradeManager == null)
                playerUpgradeManager = player.GetComponent<PlayerUpgradeManager>();

            if (playerAttack == null)
                playerAttack = player.GetComponent<PlayerAttack>();

            if (playerHealth == null)
                playerHealth = player.GetComponent<PlayerHealth>();

            if (playerMovement == null)
                playerMovement = player.GetComponent<PlayerMovement>();

            if (playerDash == null)
                playerDash = player.GetComponent<PlayerDash>();

            if (resourceManager == null)
                resourceManager = player.GetComponent<ResourceManager>();
        }

        if (companionUpgrade == null)
            companionUpgrade = FindFirstObjectByType<CompanionUpgrade>();

        if (companionCombat == null)
            companionCombat = FindFirstObjectByType<CompanionCombat>();
    }

    private void Update()
    {
        UpdateUI();
    }

    public void UpgradeAttack()
    {
        if (playerUpgradeManager != null)
            playerUpgradeManager.UpgradeAttack();

        UpdateUI();
    }

    public void UpgradeHealth()
    {
        if (playerUpgradeManager != null)
            playerUpgradeManager.UpgradeHealth();

        UpdateUI();
    }

    public void UpgradeMoveSpeed()
    {
        if (playerUpgradeManager != null)
            playerUpgradeManager.UpgradeMoveSpeed();

        UpdateUI();
    }

    public void UpgradeJumpHeight()
    {
        if (playerUpgradeManager != null)
            playerUpgradeManager.UpgradeJumpHeight();

        UpdateUI();
    }

    public void UpgradeDashSpeed()
    {
        if (playerUpgradeManager != null)
            playerUpgradeManager.UpgradeDashSpeed();

        UpdateUI();
    }

    public void UpgradeDashCooldown()
    {
        if (playerUpgradeManager != null)
            playerUpgradeManager.UpgradeDashCooldown();

        UpdateUI();
    }

    public void UpgradeAttackCooldown()
    {
        if (playerUpgradeManager != null)
            playerUpgradeManager.UpgradeAttackCooldown();

        UpdateUI();
    }

    // 兼容旧按钮绑定，不执行任何升级。
    public void UpgradeAttackRange()
    {
    }

    public void UpgradeCompanionAttack()
    {
        if (companionUpgrade != null)
            companionUpgrade.UpgradeAttack(resourceManager);

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (resourceText != null && resourceManager != null)
        {
            resourceText.text =
                "Scrap: " + resourceManager.scrap +
                "\nEnergy: " + resourceManager.energy;
        }

        if (playerUpgradeManager != null)
        {
            if (playerAttack != null)
            {
                SetStat(
                    attackText,
                    "Attack",
                    playerAttack.attackDamage,
                    playerUpgradeManager.attackUpgradeCost);

                SetStat(
                    attackCooldownText,
                    "Attack Cooldown",
                    playerAttack.attackCooldown,
                    playerUpgradeManager.attackCooldownUpgradeCost);

                if (attackRangeText != null)
                {
                    attackRangeText.text =
                        "Range: " + playerAttack.attackRange.ToString("0.##") +
                        "\nControlled by weapon";
                }
            }

            if (playerHealth != null)
            {
                SetStat(
                    healthText,
                    "Health",
                    playerHealth.maxHealth,
                    playerUpgradeManager.healthUpgradeCost);
            }

            if (playerMovement != null)
            {
                SetStat(
                    moveSpeedText,
                    "Move Speed",
                    playerMovement.moveSpeed,
                    playerUpgradeManager.moveSpeedUpgradeCost);

                SetStat(
                    jumpHeightText,
                    "Jump Height",
                    playerMovement.jumpHeight,
                    playerUpgradeManager.jumpHeightUpgradeCost);
            }

            if (playerDash != null)
            {
                SetStat(
                    dashSpeedText,
                    "Dash Speed",
                    playerDash.dashSpeed,
                    playerUpgradeManager.dashSpeedUpgradeCost);

                SetStat(
                    dashCooldownText,
                    "Dash Cooldown",
                    playerDash.dashCooldown,
                    playerUpgradeManager.dashCooldownUpgradeCost);
            }
        }

        if (companionCombat != null && companionUpgrade != null)
        {
            SetStat(
                companionAttackText,
                "Companion Attack",
                companionCombat.attackDamage,
                companionUpgrade.attackUpgradeCost,
                "Energy");
        }
    }

    private static void SetStat(
        TMP_Text target,
        string label,
        float value,
        int cost,
        string currency = "Scrap")
    {
        if (target == null) return;

        target.text =
            label + ": " + value.ToString("0.##") +
            "\nCost: " + cost + " " + currency;
    }

    public void CloseUI()
    {
        foreach (UpgradeConsole console in
                 FindObjectsByType<UpgradeConsole>(FindObjectsSortMode.None))
        {
            if (console.upgradeUI != gameObject) continue;

            console.CloseUpgradeUI();
            return;
        }

        gameObject.SetActive(false);
    }
}