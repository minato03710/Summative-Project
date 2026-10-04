using UnityEngine;
using UnityEngine.Serialization;

public enum WeaponType
{
    Unarmed = 0,
    StoneGlove = 1,
    SnippingClaws = 2,
    ScorpionTail = 3,
    SwordFish = 4,
    StandardGun = 5,
    EnergyMachineGun = 6,
    BatteryAcidProjectiles = 7
}

[System.Serializable]
public class WeaponData
{
    public string weaponName;

    [FormerlySerializedAs("stoneCost")]
    [Min(0)] public int scrapCost;
    [Min(0)] public int batteryCost;
    [Min(0)] public int energyCost;

    [Header("Melee Damage and Speed")]
    [Min(0)] public float damageMultiplier = 1f;
    [Min(0.01f)] public float attackSpeedMultiplier = 1f;

    public bool useFixedAttackInterval;
    [Min(0.001f)] public float attackCooldown = 1.2f;

    [Header("Melee Range")]
    [Min(0.01f)] public float rangeMultiplier = 1f;
    [Min(0)] public float extraRange;

    public bool useCustomRange;
    [Min(0)] public float customAttackRange = 2f;
    [Min(0.01f)] public float customAttackRadius = 1.2f;

    [Header("Ranged")]
    public bool isRanged;

    // 留空时使用 PlayerAttack 中的默认玩家子弹。
    public PlayerBullet bulletPrefab;

    [Min(0)] public float bulletDamage = 20f;
    [Min(1)] public int magazineSize = 5;
    [Min(0.01f)] public float shotInterval = 0.25f;
    [Min(0.01f)] public float reloadDuration = 3f;
    [Min(0.01f)] public float bulletSpeed = 20f;
    [Min(0.01f)] public float bulletLifetime = 4f;

    [Header("Poison")]
    [Min(0)] public float poisonDamagePerSecond;
    [Min(0)] public int poisonDurationSeconds;

    public float GetAttackInterval(float baseCooldown)
    {
        if (isRanged)
            return Mathf.Max(0.01f, shotInterval);

        return Mathf.Max(
            0.001f,
            useFixedAttackInterval
                ? attackCooldown
                : baseCooldown / Mathf.Max(0.01f, attackSpeedMultiplier));
    }
}