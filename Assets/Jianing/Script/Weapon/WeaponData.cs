using UnityEngine;

[System.Serializable]
public class WeaponData
{
    public string weaponName;


[Header("Cost")]
    public int stoneCost;

    [Header("Attack")]
    public float damageMultiplier = 1f;
    public float attackCooldown = 0.5f;


}
