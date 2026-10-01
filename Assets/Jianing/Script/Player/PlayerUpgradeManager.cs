using UnityEngine;

public class PlayerUpgradeManager : MonoBehaviour
{
    [Header("Attack Upgrade - Scrap")]
    public float attackUpgradeAmount = 1f;
    [Min(0)] public int attackUpgradeCost = 10;

    [Header("Health Upgrade - Scrap")]
    public float healthUpgradeAmount = 10f;
    [Min(0)] public int healthUpgradeCost = 15;

    [Header("Move Speed Upgrade - Scrap")]
    public float moveSpeedUpgradeAmount = 0.5f;
    [Min(0)] public int moveSpeedUpgradeCost = 15;

    [Header("Jump Height Upgrade - Scrap")]
    public float jumpHeightUpgradeAmount = 0.5f;
    [Min(0)] public int jumpHeightUpgradeCost = 15;

    [Header("Dash Speed Upgrade - Scrap")]
    public float dashSpeedUpgradeAmount = 1f;
    [Min(0)] public int dashSpeedUpgradeCost = 15;

    [Header("Dash Cooldown Upgrade - Scrap")]
    public float dashCooldownReduction = 0.1f;
    public float minimumDashCooldown = 0.2f;
    [Min(0)] public int dashCooldownUpgradeCost = 20;

    [Header("Attack Cooldown Upgrade - Scrap")]
    public float attackCooldownReduction = 0.05f;
    public float minimumAttackCooldown = 0.1f;
    [Min(0)] public int attackCooldownUpgradeCost = 20;

    private PlayerAttack playerAttack;
    private PlayerHealth playerHealth;
    private PlayerMovement playerMovement;
    private PlayerDash playerDash;
    private ResourceManager resourceManager;

    private void Awake()
    {
        playerAttack = GetComponent<PlayerAttack>();
        playerHealth = GetComponent<PlayerHealth>();
        playerMovement = GetComponent<PlayerMovement>();
        playerDash = GetComponent<PlayerDash>();
        resourceManager = GetComponent<ResourceManager>();
    }

    public bool UpgradeAttack()
    {
        if (playerAttack == null || !PayScrap(attackUpgradeCost))
            return false;

        playerAttack.baseAttackDamage += attackUpgradeAmount;
        playerAttack.RefreshAttackStats();
        return true;
    }

    public bool UpgradeHealth()
    {
        if (playerHealth == null || !PayScrap(healthUpgradeCost))
            return false;

        playerHealth.maxHealth += healthUpgradeAmount;
        playerHealth.Heal(healthUpgradeAmount);
        return true;
    }

    public bool UpgradeMoveSpeed()
    {
        if (playerMovement == null || !PayScrap(moveSpeedUpgradeCost))
            return false;

        playerMovement.moveSpeed += moveSpeedUpgradeAmount;
        return true;
    }

    public bool UpgradeJumpHeight()
    {
        if (playerMovement == null || !PayScrap(jumpHeightUpgradeCost))
            return false;

        playerMovement.jumpHeight += jumpHeightUpgradeAmount;
        return true;
    }

    public bool UpgradeDashSpeed()
    {
        if (playerDash == null || !PayScrap(dashSpeedUpgradeCost))
            return false;

        playerDash.dashSpeed += dashSpeedUpgradeAmount;
        return true;
    }

    public bool UpgradeDashCooldown()
    {
        if (playerDash == null ||
            playerDash.dashCooldown <= minimumDashCooldown)
            return false;

        if (!PayScrap(dashCooldownUpgradeCost))
            return false;

        playerDash.dashCooldown = Mathf.Max(
            minimumDashCooldown,
            playerDash.dashCooldown - dashCooldownReduction);

        return true;
    }

    public bool UpgradeAttackCooldown()
    {
        if (playerAttack == null ||
            playerAttack.baseAttackCooldown <= minimumAttackCooldown)
            return false;

        if (!PayScrap(attackCooldownUpgradeCost))
            return false;

        playerAttack.baseAttackCooldown = Mathf.Max(
            minimumAttackCooldown,
            playerAttack.baseAttackCooldown - attackCooldownReduction);

        playerAttack.RefreshAttackStats();
        return true;
    }

    // 兼容旧调用：不扣资源，也不改变攻击范围。
    public bool UpgradeAttackRange()
    {
        return false;
    }

    private bool PayScrap(int cost)
    {
        if (resourceManager == null) return false;

        bool paid = resourceManager.RemoveResource(
            ResourcePickup.ResourceType.Scrap, cost);

        if (!paid)
            Debug.Log("Not enough Scrap, or invalid cost.");

        return paid;
    }
}