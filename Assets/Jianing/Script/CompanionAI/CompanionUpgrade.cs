using UnityEngine;

public class CompanionUpgrade : MonoBehaviour
{
    [Header("Attack Upgrade - Energy")]
    public float attackUpgradeAmount = 1f;
    [Min(0)] public int attackUpgradeCost = 10;

    private CompanionCombat companionCombat;

    public bool UpgradeAttack(ResourceManager resourceManager)
    {
        if (resourceManager == null) return false;

        if (companionCombat == null)
            companionCombat = GetComponent<CompanionCombat>();

        if (companionCombat == null) return false;

        if (!resourceManager.RemoveResource(
                ResourcePickup.ResourceType.Energy,
                attackUpgradeCost))
        {
            Debug.Log("Not enough Energy, or invalid cost.");
            return false;
        }

        companionCombat.attackDamage += attackUpgradeAmount;
        return true;
    }
}