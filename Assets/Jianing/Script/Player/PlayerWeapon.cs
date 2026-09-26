using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(PlayerAttack), typeof(ResourceManager))]
public class PlayerWeapon : MonoBehaviour
{
    [Header("Stone Glove")]
    [Min(0)] public int stoneGloveCost = 5;
    [Min(0.01f)] public float stoneGloveDamageMultiplier = 1.5f;
    [Min(0.01f)] public float stoneGloveAttackCooldown = 1.2f;

    private PlayerAttack playerAttack;
    private ResourceManager resourceManager;
    private bool hasStoneGlove;
    private bool stoneGloveEquipped;

    public int StoneCount
    {
        get
        {
            CacheComponents();
            return resourceManager != null ? resourceManager.stone : 0;
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

        // Once owned, equipping is free. Never charge for the same weapon twice.
        if (hasStoneGlove)
        {
            EquipStoneGlove();
            return true;
        }

        if (!resourceManager.RemoveResource(
                ResourcePickup.ResourceType.Stone, Mathf.Max(0, stoneGloveCost)))
        {
            return false;
        }

        hasStoneGlove = true;
        EquipStoneGlove();
        return true;
    }

    public void EquipStoneGlove()
    {
        if (!hasStoneGlove)
            return;

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

    // Used by the project's existing scene-to-scene data loader.
    public void RestoreWeaponState(bool owned, bool equipped)
    {
        hasStoneGlove = owned;
        stoneGloveEquipped = owned && equipped;
        ApplyEquipment();
    }

    private void ApplyEquipment()
    {
        CacheComponents();
        if (playerAttack == null)
            return;

        if (stoneGloveEquipped)
        {
            playerAttack.ApplyWeaponStats(
                stoneGloveDamageMultiplier, stoneGloveAttackCooldown);
        }
        else
        {
            playerAttack.RemoveWeaponStats();
        }
    }
}
