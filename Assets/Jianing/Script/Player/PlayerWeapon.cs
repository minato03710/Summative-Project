using UnityEngine;
using UnityEngine.Serialization;

[DisallowMultipleComponent]
[RequireComponent(typeof(PlayerAttack), typeof(ResourceManager))]
public class PlayerWeapon : MonoBehaviour
{
    [Header("Stone Glove")]
    [FormerlySerializedAs("stoneGloveCost")]
    [Min(0)] public int stoneGloveScrapCost = 5;

    [Min(0.01f)] public float stoneGloveDamageMultiplier = 1.5f;
    [Min(0.01f)] public float stoneGloveAttackCooldown = 1.2f;

    private PlayerAttack playerAttack;
    private ResourceManager resourceManager;
    private bool hasStoneGlove;
    private bool stoneGloveEquipped;

    public int ScrapCount
    {
        get
        {
            CacheComponents();
            return resourceManager != null ? resourceManager.scrap : 0;
        }
    }

    private void Awake()
    {
        CacheComponents();
    }

    private void Start()
    {
        ApplyEquipment();
    }

    private void CacheComponents()
    {
        if (playerAttack == null)
            playerAttack = GetComponent<PlayerAttack>();

        if (resourceManager == null)
            resourceManager = GetComponent<ResourceManager>();
    }

    public bool BuyStoneGlove()
    {
        CacheComponents();

        if (playerAttack == null || resourceManager == null)
            return false;

        if (hasStoneGlove)
        {
            EquipStoneGlove();
            return true;
        }

        if (!resourceManager.RemoveResource(
                ResourcePickup.ResourceType.Scrap,
                stoneGloveScrapCost))
            return false;

        hasStoneGlove = true;
        EquipStoneGlove();
        return true;
    }

    public void EquipStoneGlove()
    {
        if (!hasStoneGlove) return;

        stoneGloveEquipped = true;
        ApplyEquipment();
    }

    public void UnequipStoneGlove()
    {
        stoneGloveEquipped = false;
        ApplyEquipment();
    }

    public bool HasStoneGlove()
    {
        return hasStoneGlove;
    }

    public bool IsStoneGloveEquipped()
    {
        return stoneGloveEquipped;
    }

    public void RestoreWeaponState(bool owned, bool equipped)
    {
        hasStoneGlove = owned;
        stoneGloveEquipped = owned && equipped;
        ApplyEquipment();
    }

    private void ApplyEquipment()
    {
        CacheComponents();

        if (playerAttack == null) return;

        if (stoneGloveEquipped)
        {
            playerAttack.ApplyWeaponStats(
                stoneGloveDamageMultiplier,
                stoneGloveAttackCooldown);
        }
        else
        {
            playerAttack.RemoveWeaponStats();
        }
    }
}