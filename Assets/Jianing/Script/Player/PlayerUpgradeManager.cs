using UnityEngine;

public class PlayerUpgradeManager : MonoBehaviour
{
// =========================================================
// Attack Upgrade
// =========================================================

[Header("Attack Upgrade")]

    public float attackUpgradeAmount = 1f;
    public int attackUpgradeCost = 10;


    // =========================================================
    // Health Upgrade
    // =========================================================

    [Header("Health Upgrade")]

    public float healthUpgradeAmount = 10f;
    public int healthUpgradeCost = 15;


    // =========================================================
    // Move Speed Upgrade
    // =========================================================

    [Header("Move Speed Upgrade")]

    public float moveSpeedUpgradeAmount = 0.5f;
    public int moveSpeedUpgradeCost = 15;


    // =========================================================
    // Jump Height Upgrade
    // =========================================================

    [Header("Jump Height Upgrade")]

    public float jumpHeightUpgradeAmount = 0.5f;
    public int jumpHeightUpgradeCost = 15;


    // =========================================================
    // Dash Speed Upgrade
    // =========================================================

    [Header("Dash Speed Upgrade")]

    public float dashSpeedUpgradeAmount = 1f;
    public int dashSpeedUpgradeCost = 15;


    // =========================================================
    // Dash Cooldown Upgrade
    // =========================================================

    [Header("Dash Cooldown Upgrade")]

    public float dashCooldownReduction = 0.1f;
    public float minimumDashCooldown = 0.2f;
    public int dashCooldownUpgradeCost = 20;


    // =========================================================
    // Attack Cooldown Upgrade
    // =========================================================

    [Header("Attack Cooldown Upgrade")]

    public float attackCooldownReduction = 0.05f;
    public float minimumAttackCooldown = 0.1f;
    public int attackCooldownUpgradeCost = 20;


    // =========================================================
    // Attack Range Upgrade
    // =========================================================

    [Header("Attack Range Upgrade")]

    public float attackRangeUpgradeAmount = 0.2f;
    public int attackRangeUpgradeCost = 15;


    // =========================================================
    // Components
    // =========================================================

    private PlayerAttack playerAttack;
    private PlayerHealth playerHealth;
    private PlayerMovement playerMovement;
    private PlayerDash playerDash;
    private ResourceManager resourceManager;


    // =========================================================
    // Start
    // =========================================================

    void Start()
    {
        playerAttack =
            GetComponent<PlayerAttack>();

        playerHealth =
            GetComponent<PlayerHealth>();

        playerMovement =
            GetComponent<PlayerMovement>();

        playerDash =
            GetComponent<PlayerDash>();

        resourceManager =
            GetComponent<ResourceManager>();
    }


    // =========================================================
    // Attack
    // =========================================================

    public bool UpgradeAttack()
    {
        if (resourceManager == null || playerAttack == null)
            return false;

        if (!PayGold(attackUpgradeCost))
            return false;

        // 升级基础攻击力
        playerAttack.baseAttackDamage +=
            attackUpgradeAmount;

        // 如果当前没有特殊武器，
        // 最终攻击力直接等于基础攻击力
        playerAttack.attackDamage =
            playerAttack.baseAttackDamage;

        Debug.Log(
            "Player Attack upgraded! " +
            "Base Attack = " +
            playerAttack.baseAttackDamage +
            " | Current Attack = " +
            playerAttack.attackDamage
        );

        return true;

    }


    // =========================================================
    // Health
    // =========================================================

    public bool UpgradeHealth()
    {
        if (resourceManager == null ||
            playerHealth == null)
            return false;

        if (!PayGold(healthUpgradeCost))
            return false;

        playerHealth.maxHealth +=
            healthUpgradeAmount;

        playerHealth.Heal(
            healthUpgradeAmount
        );

        Debug.Log(
            "Player Health upgraded! " +
            "Max Health = " +
            playerHealth.maxHealth
        );

        return true;
    }


    // =========================================================
    // Move Speed
    // =========================================================

    public bool UpgradeMoveSpeed()
    {
        if (resourceManager == null ||
            playerMovement == null)
            return false;

        if (!PayGold(moveSpeedUpgradeCost))
            return false;

        playerMovement.moveSpeed +=
            moveSpeedUpgradeAmount;

        Debug.Log(
            "Move Speed upgraded! " +
            "Current Speed = " +
            playerMovement.moveSpeed
        );

        return true;
    }


    // =========================================================
    // Jump Height
    // =========================================================

    public bool UpgradeJumpHeight()
    {
        if (resourceManager == null ||
            playerMovement == null)
            return false;

        if (!PayGold(jumpHeightUpgradeCost))
            return false;

        playerMovement.jumpHeight +=
            jumpHeightUpgradeAmount;

        Debug.Log(
            "Jump Height upgraded! " +
            "Current Jump Height = " +
            playerMovement.jumpHeight
        );

        return true;
    }


    // =========================================================
    // Dash Speed
    // =========================================================

    public bool UpgradeDashSpeed()
    {
        if (resourceManager == null ||
            playerDash == null)
            return false;

        if (!PayGold(dashSpeedUpgradeCost))
            return false;

        playerDash.dashSpeed +=
            dashSpeedUpgradeAmount;

        Debug.Log(
            "Dash Speed upgraded! " +
            "Current Dash Speed = " +
            playerDash.dashSpeed
        );

        return true;
    }


    // =========================================================
    // Dash Cooldown
    // =========================================================

    public bool UpgradeDashCooldown()
    {
        if (resourceManager == null ||
            playerDash == null)
            return false;

        if (playerDash.dashCooldown <=
            minimumDashCooldown)
        {
            Debug.Log(
                "Dash Cooldown already at minimum!"
            );

            return false;
        }

        if (!PayGold(dashCooldownUpgradeCost))
            return false;

        playerDash.dashCooldown =
            Mathf.Max(
                minimumDashCooldown,
                playerDash.dashCooldown -
                dashCooldownReduction
            );

        Debug.Log(
            "Dash Cooldown upgraded! " +
            "Current Cooldown = " +
            playerDash.dashCooldown
        );

        return true;
    }


    // =========================================================
    // Attack Cooldown
    // =========================================================

    public bool UpgradeAttackCooldown()
    {
        if (resourceManager == null ||
            playerAttack == null)
            return false;

        if (playerAttack.attackCooldown <=
            minimumAttackCooldown)
        {
            Debug.Log(
                "Attack Cooldown already at minimum!"
            );

            return false;
        }

        if (!PayGold(attackCooldownUpgradeCost))
            return false;

        playerAttack.attackCooldown =
            Mathf.Max(
                minimumAttackCooldown,
                playerAttack.attackCooldown -
                attackCooldownReduction
            );

        Debug.Log(
            "Attack Cooldown upgraded! " +
            "Current Cooldown = " +
            playerAttack.attackCooldown
        );

        return true;
    }


    // =========================================================
    // Attack Range
    // =========================================================

    public bool UpgradeAttackRange()
    {
        if (resourceManager == null ||
            playerAttack == null)
            return false;

        if (!PayGold(attackRangeUpgradeCost))
            return false;

        playerAttack.attackRange +=
            attackRangeUpgradeAmount;

        Debug.Log(
            "Attack Range upgraded! " +
            "Current Range = " +
            playerAttack.attackRange
        );

        return true;
    }


    // =========================================================
    // Pay Gold
    // =========================================================

    bool PayGold(int cost)
    {
        if (resourceManager.gold < cost)
        {
            Debug.Log(
                "Not enough Gold!"
            );

            return false;
        }

        resourceManager.RemoveResource(
            ResourcePickup.ResourceType.Gold,
            cost
        );

        return true;
    }


}
