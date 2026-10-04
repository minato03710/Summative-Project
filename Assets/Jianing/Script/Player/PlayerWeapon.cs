using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[DisallowMultipleComponent]
[RequireComponent(typeof(PlayerAttack), typeof(ResourceManager))]
public class PlayerWeapon : MonoBehaviour
{
    [Header("Stone Glove - Existing Settings")]
    [FormerlySerializedAs("stoneGloveCost")]
    [Min(0)] public int stoneGloveScrapCost = 5;

    [Min(0)] public float stoneGloveDamageMultiplier = 1.5f;
    [Min(0.001f)] public float stoneGloveAttackCooldown = 1.2f;

    public WeaponData snippingClaws = new WeaponData
    {
        weaponName = "Snipping Claws",
        scrapCost = 5,
        batteryCost = 5,
        damageMultiplier = 0.8f,
        attackSpeedMultiplier = 3f,
        rangeMultiplier = 0.8f
    };

    public WeaponData scorpionTail = new WeaponData
    {
        weaponName = "Scorpion Tail",
        scrapCost = 5,
        batteryCost = 10,
        energyCost = 5,
        damageMultiplier = 1f,
        rangeMultiplier = 1.5f,
        poisonDamagePerSecond = 20f,
        poisonDurationSeconds = 3
    };

    public WeaponData swordFish = new WeaponData
    {
        weaponName = "Sword(Fish)",
        scrapCost = 15,
        energyCost = 5,
        damageMultiplier = 1.5f,
        attackSpeedMultiplier = 11.5f,
        rangeMultiplier = 1.5f
    };

    [Header("Ranged Weapons")]
    public WeaponData standardGun = new WeaponData
    {
        weaponName = "Standard Gun",
        scrapCost = 20,
        isRanged = true,
        bulletDamage = 20f,
        magazineSize = 5,
        shotInterval = 0.25f,
        reloadDuration = 3f,
        bulletSpeed = 20f
    };

    public WeaponData energyMachineGun = new WeaponData
    {
        weaponName = "Energy Machine Gun",
        scrapCost = 20,
        energyCost = 20,
        isRanged = true,
        bulletDamage = 20f,
        magazineSize = 15,
        shotInterval = 0.1f,
        reloadDuration = 6f,
        bulletSpeed = 35f
    };

    public WeaponData batteryAcidProjectiles = new WeaponData
    {
        weaponName = "Battery Acid Projectiles",
        scrapCost = 30,
        batteryCost = 30,
        isRanged = true,
        bulletDamage = 20f,
        magazineSize = 4,
        shotInterval = 0.25f,
        reloadDuration = 2f,
        bulletSpeed = 35f,
        poisonDamagePerSecond = 20f,
        poisonDurationSeconds = 3
    };

    private PlayerAttack playerAttack;
    private ResourceManager resources;

    private readonly HashSet<WeaponType> ownedWeapons =
        new HashSet<WeaponType>();

    public WeaponType EquippedWeapon { get; private set; } =
        WeaponType.Unarmed;

    public int ScrapCount
    {
        get
        {
            Cache();
            return resources != null ? resources.scrap : 0;
        }
    }

    public int BatteryCount
    {
        get
        {
            Cache();
            return resources != null ? resources.battery : 0;
        }
    }

    public int EnergyCount
    {
        get
        {
            Cache();
            return resources != null ? resources.energy : 0;
        }
    }

    private void Awake()
    {
        Cache();
    }

    private void Start()
    {
        ApplyEquipment();
    }

    private void Cache()
    {
        if (playerAttack == null)
            playerAttack = GetComponent<PlayerAttack>();

        if (resources == null)
            resources = GetComponent<ResourceManager>();
    }

    public WeaponData GetWeaponData(WeaponType type)
    {
        switch (type)
        {
            case WeaponType.StoneGlove:
                return new WeaponData
                {
                    weaponName = "Stone Glove",
                    scrapCost = stoneGloveScrapCost,
                    damageMultiplier = stoneGloveDamageMultiplier,
                    useFixedAttackInterval = true,
                    attackCooldown = stoneGloveAttackCooldown,
                    rangeMultiplier = 1f
                };

            case WeaponType.SnippingClaws:
                return snippingClaws;

            case WeaponType.ScorpionTail:
                return scorpionTail;

            case WeaponType.SwordFish:
                return swordFish;

            case WeaponType.StandardGun:
                return standardGun;

            case WeaponType.EnergyMachineGun:
                return energyMachineGun;

            case WeaponType.BatteryAcidProjectiles:
                return batteryAcidProjectiles;

            default:
                return null;
        }
    }

    public bool IsOwned(WeaponType type)
    {
        return type == WeaponType.Unarmed ||
               ownedWeapons.Contains(type);
    }

    public bool BuyOrEquip(WeaponType type)
    {
        Cache();

        if (playerAttack == null || resources == null)
            return false;

        if (IsOwned(type))
            return Equip(type);

        WeaponData data = GetWeaponData(type);

        if (data == null ||
            data.scrapCost < 0 ||
            data.batteryCost < 0 ||
            data.energyCost < 0)
            return false;

        // 所有材料都足够后才扣款。
        if (resources.scrap < data.scrapCost ||
            resources.battery < data.batteryCost ||
            resources.energy < data.energyCost)
            return false;

        resources.RemoveResource(
            ResourcePickup.ResourceType.Scrap,
            data.scrapCost);

        resources.RemoveResource(
            ResourcePickup.ResourceType.Battery,
            data.batteryCost);

        resources.RemoveResource(
            ResourcePickup.ResourceType.Energy,
            data.energyCost);

        ownedWeapons.Add(type);
        return Equip(type);
    }

    public bool Equip(WeaponType type)
    {
        Cache();

        if (playerAttack == null || !IsOwned(type))
            return false;

        if (type != WeaponType.Unarmed &&
            GetWeaponData(type) == null)
            return false;

        EquippedWeapon = type;
        ApplyEquipment();
        return true;
    }

    private void ApplyEquipment()
    {
        Cache();

        if (playerAttack != null)
        {
            playerAttack.ApplyWeaponStats(
                GetWeaponData(EquippedWeapon));
        }
    }

    public List<WeaponType> GetOwnedWeapons()
    {
        return new List<WeaponType>(ownedWeapons);
    }

    public void RestoreWeapons(
        List<WeaponType> owned,
        WeaponType equipped)
    {
        ownedWeapons.Clear();

        if (owned != null)
        {
            foreach (WeaponType type in owned)
            {
                if (type != WeaponType.Unarmed &&
                    GetWeaponData(type) != null)
                {
                    ownedWeapons.Add(type);
                }
            }
        }

        EquippedWeapon = IsOwned(equipped)
            ? equipped
            : WeaponType.Unarmed;

        ApplyEquipment();
    }

    public bool BuyStoneGlove()
    {
        return BuyOrEquip(WeaponType.StoneGlove);
    }

    public void EquipStoneGlove()
    {
        Equip(WeaponType.StoneGlove);
    }

    public void UnequipStoneGlove()
    {
        Equip(WeaponType.Unarmed);
    }

    public bool HasStoneGlove()
    {
        return IsOwned(WeaponType.StoneGlove);
    }

    public bool IsStoneGloveEquipped()
    {
        return EquippedWeapon == WeaponType.StoneGlove;
    }
}