using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance;


// =========================
// Resources
// =========================

[Header("Resources")]
    public int wood;
    public int stone;
    public int food;
    public int gold;


    // =========================
    // Player Stats
    // =========================

    [Header("Player Stats")]

    public float attackDamage;
    public float maxHealth;

    public float moveSpeed;
    public float jumpHeight;

    public float dashSpeed;
    public float dashCooldown;

    public float attackCooldown;
    public float attackRange;


    // =========================
    // Companion Stats
    // =========================

    [Header("Companion Stats")]
    public float companionAttackDamage;


    // =========================
    // Save State
    // =========================

    private bool hasPlayerData = false;
    private bool hasCompanionData = false;


    // =========================
    // Singleton
    // =========================

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }


    // =========================================================
    // SAVE PLAYER DATA
    // =========================================================

    public void SavePlayerData(
        ResourceManager resourceManager,
        PlayerAttack playerAttack,
        PlayerHealth playerHealth,
        PlayerMovement playerMovement,
        PlayerDash playerDash
    )
    {
        // Resources

        if (resourceManager != null)
        {
            wood = resourceManager.wood;
            stone = resourceManager.stone;
            food = resourceManager.food;
            gold = resourceManager.gold;
        }


        // Player Attack

        if (playerAttack != null)
        {
            attackDamage =
                playerAttack.attackDamage;

            attackCooldown =
                playerAttack.attackCooldown;

            attackRange =
                playerAttack.attackRange;
        }


        // Player Health

        if (playerHealth != null)
        {
            maxHealth =
                playerHealth.maxHealth;
        }


        // Player Movement

        if (playerMovement != null)
        {
            moveSpeed =
                playerMovement.moveSpeed;

            jumpHeight =
                playerMovement.jumpHeight;
        }


        // Player Dash

        if (playerDash != null)
        {
            dashSpeed =
                playerDash.dashSpeed;

            dashCooldown =
                playerDash.dashCooldown;
        }


        hasPlayerData = true;


        Debug.Log(
            "Player Data Saved | " +

            "Wood: " + wood +
            " | Stone: " + stone +
            " | Food: " + food +
            " | Gold: " + gold +

            " | Attack: " + attackDamage +
            " | Max Health: " + maxHealth +

            " | Move Speed: " + moveSpeed +
            " | Jump Height: " + jumpHeight +

            " | Dash Speed: " + dashSpeed +
            " | Dash Cooldown: " + dashCooldown +

            " | Attack Cooldown: " + attackCooldown +
            " | Attack Range: " + attackRange
        );
    }


    // =========================================================
    // LOAD PLAYER DATA
    // =========================================================

    public void LoadPlayerData(
        ResourceManager resourceManager,
        PlayerAttack playerAttack,
        PlayerHealth playerHealth,
        PlayerMovement playerMovement,
        PlayerDash playerDash
    )
    {
        // 没有保存过数据
        // 使用场景自己的初始值

        if (!hasPlayerData)
        {
            Debug.Log(
                "No saved player data. " +
                "Using scene default values."
            );

            return;
        }


        // Resources

        if (resourceManager != null)
        {
            resourceManager.wood = wood;
            resourceManager.stone = stone;
            resourceManager.food = food;
            resourceManager.gold = gold;
        }


        // Player Attack

        if (playerAttack != null)
        {
            playerAttack.attackDamage =
                attackDamage;

            playerAttack.attackCooldown =
                attackCooldown;

            playerAttack.attackRange =
                attackRange;
        }


        // Player Health

        if (playerHealth != null)
        {
            playerHealth.maxHealth =
                maxHealth;
        }


        // Player Movement

        if (playerMovement != null)
        {
            playerMovement.moveSpeed =
                moveSpeed;

            playerMovement.jumpHeight =
                jumpHeight;
        }


        // Player Dash

        if (playerDash != null)
        {
            playerDash.dashSpeed =
                dashSpeed;

            playerDash.dashCooldown =
                dashCooldown;
        }


        Debug.Log(
            "Player Data Loaded | " +

            "Wood: " + wood +
            " | Stone: " + stone +
            " | Food: " + food +
            " | Gold: " + gold +

            " | Attack: " + attackDamage +
            " | Max Health: " + maxHealth +

            " | Move Speed: " + moveSpeed +
            " | Jump Height: " + jumpHeight +

            " | Dash Speed: " + dashSpeed +
            " | Dash Cooldown: " + dashCooldown +

            " | Attack Cooldown: " + attackCooldown +
            " | Attack Range: " + attackRange
        );
    }


    // =========================================================
    // SAVE COMPANION DATA
    // =========================================================

    public void SaveCompanionData(
        CompanionCombat companionCombat
    )
    {
        if (companionCombat == null)
        {
            Debug.LogWarning(
                "SaveCompanionData: " +
                "找不到 CompanionCombat!"
            );

            return;
        }

        companionAttackDamage =
            companionCombat.attackDamage;

        hasCompanionData = true;

        Debug.Log(
            "Companion Data Saved | " +
            "Attack: " +
            companionAttackDamage
        );
    }


    // =========================================================
    // LOAD COMPANION DATA
    // =========================================================

    public void LoadCompanionData(
        CompanionCombat companionCombat
    )
    {
        if (companionCombat == null)
        {
            Debug.LogWarning(
                "LoadCompanionData: " +
                "找不到 CompanionCombat!"
            );

            return;
        }

        if (!hasCompanionData)
        {
            Debug.Log(
                "No saved companion data. " +
                "Using scene default values."
            );

            return;
        }

        companionCombat.attackDamage =
            companionAttackDamage;

        Debug.Log(
            "Companion Data Loaded | " +
            "Attack: " +
            companionAttackDamage
        );
    }


}
