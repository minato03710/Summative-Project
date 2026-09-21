using UnityEngine;

public class CompanionUpgrade : MonoBehaviour
{
    [Header("Attack Upgrade")]
    public float attackUpgradeAmount = 1f;
    public int attackUpgradeCost = 10;


private CompanionCombat companionCombat;

    void Start()
    {
        companionCombat =
            GetComponent<CompanionCombat>();
    }

    public bool UpgradeAttack(
        ResourceManager resourceManager
    )
    {
        if (resourceManager == null)
        {
            Debug.LogWarning(
                "找不到 ResourceManager!"
            );

            return false;
        }

        if (companionCombat == null)
        {
            companionCombat =
                GetComponent<CompanionCombat>();
        }

        if (companionCombat == null)
        {
            Debug.LogWarning(
                "找不到 CompanionCombat!"
            );

            return false;
        }

        if (resourceManager.gold <
            attackUpgradeCost)
        {
            Debug.Log(
                "Not enough Gold!"
            );

            return false;
        }

        resourceManager.RemoveResource(
            ResourcePickup.ResourceType.Gold,
            attackUpgradeCost
        );

        companionCombat.attackDamage +=
            attackUpgradeAmount;

        Debug.Log(
            "Companion Attack upgraded! " +
            "Current Attack = " +
            companionCombat.attackDamage
        );

        return true;
    }


}

