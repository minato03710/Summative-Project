using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class WeaponData
{
    public string weaponName;

    [Header("Cost - Scrap")]
    [FormerlySerializedAs("stoneCost")]
    [Min(0)] public int scrapCost;

    [Header("Attack")]
    public float damageMultiplier = 1f;
    public float attackCooldown = 0.5f;
}