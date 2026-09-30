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
    public float attackRange;
    public bool ownsStoneGlove;
    public bool stoneGloveEquipped;

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
            attackDamage = playerAttack.baseAttackDamage;
            attackCooldown = playerAttack.baseAttackCooldown;
            attackRange = playerAttack.attackRange;

            PlayerWeapon weapon = playerAttack.GetComponent<PlayerWeapon>();

            ownsStoneGlove = weapon != null && weapon.HasStoneGlove();
            stoneGloveEquipped = weapon != null && weapon.IsStoneGloveEquipped();
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
            playerAttack.baseAttackDamage = attackDamage;
            playerAttack.baseAttackCooldown = attackCooldown;
            playerAttack.attackRange = attackRange;

            PlayerWeapon weapon = playerAttack.GetComponent<PlayerWeapon>();

            if (weapon != null)
                weapon.RestoreWeaponState(ownsStoneGlove, stoneGloveEquipped);
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