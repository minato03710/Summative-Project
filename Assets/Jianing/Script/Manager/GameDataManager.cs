using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance;

    [Header("Resources")]
    [FormerlySerializedAs("wood")] public int energy;
    [FormerlySerializedAs("stone")] public int battery;
    public int food;
    [FormerlySerializedAs("gold")] public int scrap;

    [Header("Player Stats")]
    public float attackDamage;
    public float maxHealth;
    public float moveSpeed;
    public float jumpHeight;
    public float dashSpeed;
    public float dashCooldown;
    public float attackCooldown;

    // 这里保存基础范围，不包含武器倍率。
    public float attackRange;
    public float attackRadius = 1.2f;

    public List<WeaponType> ownedWeapons =
        new List<WeaponType>();

    public WeaponType equippedWeapon = WeaponType.Unarmed;

    [Header("Companion Stats")]
    public float companionAttackDamage;

    private bool hasPlayerData;
    private bool hasCompanionData;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SavePlayerData(
        ResourceManager resourceManager,
        PlayerAttack playerAttack,
        PlayerHealth playerHealth,
        PlayerMovement playerMovement,
        PlayerDash playerDash)
    {
        if (resourceManager != null)
        {
            energy = resourceManager.energy;
            battery = resourceManager.battery;
            food = resourceManager.food;
            scrap = resourceManager.scrap;
        }

        if (playerAttack != null)
        {
            playerAttack.InitializeStats();

            attackDamage = playerAttack.baseAttackDamage;
            attackCooldown = playerAttack.baseAttackCooldown;
            attackRange = playerAttack.baseAttackRange;
            attackRadius = playerAttack.baseAttackRadius;

            PlayerWeapon weapon =
                playerAttack.GetComponent<PlayerWeapon>();

            ownedWeapons = weapon != null
                ? weapon.GetOwnedWeapons()
                : new List<WeaponType>();

            equippedWeapon = weapon != null
                ? weapon.EquippedWeapon
                : WeaponType.Unarmed;
        }

        if (playerHealth != null)
            maxHealth = playerHealth.maxHealth;

        if (playerMovement != null)
        {
            moveSpeed = playerMovement.moveSpeed;
            jumpHeight = playerMovement.jumpHeight;
        }

        if (playerDash != null)
        {
            dashSpeed = playerDash.dashSpeed;
            dashCooldown = playerDash.dashCooldown;
        }

        hasPlayerData = true;
    }

    public void LoadPlayerData(
        ResourceManager resourceManager,
        PlayerAttack playerAttack,
        PlayerHealth playerHealth,
        PlayerMovement playerMovement,
        PlayerDash playerDash)
    {
        if (!hasPlayerData) return;

        if (resourceManager != null)
        {
            resourceManager.energy = energy;
            resourceManager.battery = battery;
            resourceManager.food = food;
            resourceManager.scrap = scrap;
        }

        if (playerAttack != null)
        {
            playerAttack.InitializeStats();

            playerAttack.baseAttackDamage = attackDamage;
            playerAttack.baseAttackCooldown = attackCooldown;
            playerAttack.baseAttackRange = attackRange;
            playerAttack.baseAttackRadius = attackRadius;

            PlayerWeapon weapon =
                playerAttack.GetComponent<PlayerWeapon>();

            if (weapon != null)
                weapon.RestoreWeapons(ownedWeapons, equippedWeapon);
            else
                playerAttack.RemoveWeaponStats();
        }

        if (playerHealth != null)
            playerHealth.maxHealth = maxHealth;

        if (playerMovement != null)
        {
            playerMovement.moveSpeed = moveSpeed;
            playerMovement.jumpHeight = jumpHeight;
        }

        if (playerDash != null)
        {
            playerDash.dashSpeed = dashSpeed;
            playerDash.dashCooldown = dashCooldown;
        }
    }

    public void SaveCompanionData(CompanionCombat companionCombat)
    {
        if (companionCombat == null) return;

        companionAttackDamage = companionCombat.attackDamage;
        hasCompanionData = true;
    }

    public void LoadCompanionData(CompanionCombat companionCombat)
    {
        if (companionCombat == null || !hasCompanionData) return;

        companionCombat.attackDamage = companionAttackDamage;
    }
}