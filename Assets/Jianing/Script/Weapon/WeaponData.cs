using UnityEngine;
using UnityEngine.Serialization;

public enum WeaponType
{
    Unarmed = 0,
    StoneGlove = 1,
    SnippingClaws = 2,
    ScorpionTail = 3,
    SwordFish = 4
}

[System.Serializable]
public class WeaponData
{
    public string weaponName;

    [FormerlySerializedAs("stoneCost")]
    [Min(0)] public int scrapCost;
    [Min(0)] public int batteryCost;
    [Min(0)] public int energyCost;

    [Header("Damage and Speed")]
    [Min(0)] public float damageMultiplier = 1f;
    [Min(0.01f)] public float attackSpeedMultiplier = 1f;

    public bool useFixedAttackInterval;
    [Min(0.001f)] public float attackCooldown = 1.2f;

    [Header("Range")]
    [Min(0.01f)] public float rangeMultiplier = 1f;
    [Min(0)] public float extraRange;

    public bool useCustomRange;
    [Min(0)] public float customAttackRange = 2f;
    [Min(0.01f)] public float customAttackRadius = 1.2f;

    [Header("Poison")]
    [Min(0)] public float poisonDamagePerSecond;
    [Min(0)] public int poisonDurationSeconds;

    public float GetAttackInterval(float baseCooldown)
    {
        return Mathf.Max(
            0.001f,
            useFixedAttackInterval
                ? attackCooldown
                : baseCooldown / Mathf.Max(0.01f, attackSpeedMultiplier)
        );
    }
}