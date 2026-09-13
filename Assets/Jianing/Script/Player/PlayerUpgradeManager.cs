using UnityEngine;

public class PlayerUpgradeManager : MonoBehaviour
{
    [Header("Attack Upgrade")]
    public float attackUpgradeAmount = 1f;
    public int attackUpgradeCost = 10;


[Header("Health Upgrade")]
    public float healthUpgradeAmount = 10f;
    public int healthUpgradeCost = 15;

    private PlayerAttack playerAttack;
    private PlayerHealth playerHealth;
    private ResourceManager resourceManager;

    void Start()
    {
        playerAttack = GetComponent<PlayerAttack>();
        playerHealth = GetComponent<PlayerHealth>();
        resourceManager = GetComponent<ResourceManager>();
    }

    public bool UpgradeAttack()
    {
        if (resourceManager == null)
            return false;

        if (playerAttack == null)
            return false;

        // 检查金币
        if (resourceManager.gold < attackUpgradeCost)
        {
            Debug.Log("Not enough Gold!");
            return false;
        }

        // 扣除金币
        resourceManager.RemoveResource(
            ResourcePickup.ResourceType.Gold,
            attackUpgradeCost
        );

        // 增加攻击力
        playerAttack.attackDamage += attackUpgradeAmount;

        Debug.Log(
            "Attack upgraded! Current Attack = " +
            playerAttack.attackDamage
        );

        return true;
    }

    public bool UpgradeHealth()
    {
        if (resourceManager == null)
            return false;

        if (playerHealth == null)
            return false;

        // 检查金币
        if (resourceManager.gold < healthUpgradeCost)
        {
            Debug.Log("Not enough Gold!");
            return false;
        }

        // 扣除金币
        resourceManager.RemoveResource(
            ResourcePickup.ResourceType.Gold,
            healthUpgradeCost
        );

        // 增加最大生命值
        playerHealth.maxHealth += healthUpgradeAmount;

        // 同时恢复增加的生命值
        playerHealth.Heal(healthUpgradeAmount);

        Debug.Log(
            "Health upgraded! Max Health = " +
            playerHealth.maxHealth
        );

        return true;
    }


}

