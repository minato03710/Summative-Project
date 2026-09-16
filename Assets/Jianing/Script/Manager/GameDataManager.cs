using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance;


[Header("Resources")]
    public int wood;
    public int stone;
    public int food;
    public int gold;

    [Header("Player Stats")]
    public float attackDamage;
    public float maxHealth;

    void Awake()
    {
        // 如果已经存在管理器
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // 设置单例
        Instance = this;

        // 切换场景时不销毁
        DontDestroyOnLoad(gameObject);
    }

    public void SavePlayerData(
        ResourceManager resourceManager,
        PlayerAttack playerAttack,
        PlayerHealth playerHealth
    )
    {
        if (resourceManager != null)
        {
            wood = resourceManager.wood;
            stone = resourceManager.stone;
            food = resourceManager.food;
            gold = resourceManager.gold;
        }

        if (playerAttack != null)
        {
            attackDamage =
                playerAttack.attackDamage;
        }

        if (playerHealth != null)
        {
            maxHealth =
                playerHealth.maxHealth;
        }

        Debug.Log(
            "Game Data Saved | " +
            "Wood: " + wood +
            " | Stone: " + stone +
            " | Food: " + food +
            " | Gold: " + gold +
            " | Attack: " + attackDamage +
            " | Max Health: " + maxHealth
        );
    }

    public void LoadPlayerData(
        ResourceManager resourceManager,
        PlayerAttack playerAttack,
        PlayerHealth playerHealth
    )
    {
        if (resourceManager != null)
        {
            resourceManager.wood = wood;
            resourceManager.stone = stone;
            resourceManager.food = food;
            resourceManager.gold = gold;
        }

        if (playerAttack != null)
        {
            playerAttack.attackDamage =
                attackDamage;
        }

        if (playerHealth != null)
        {
            playerHealth.maxHealth =
                maxHealth;
        }

        Debug.Log(
            "Game Data Loaded | " +
            "Wood: " + wood +
            " | Stone: " + stone +
            " | Food: " + food +
            " | Gold: " + gold +
            " | Attack: " + attackDamage +
            " | Max Health: " + maxHealth
        );
    }


}

