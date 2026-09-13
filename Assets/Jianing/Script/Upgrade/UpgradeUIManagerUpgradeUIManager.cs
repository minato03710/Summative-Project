using UnityEngine;
using TMPro;

public class UpgradeUIManager : MonoBehaviour
{
    [Header("UI Text")]
    public TMP_Text goldText;
    public TMP_Text attackText;
    public TMP_Text healthText;


[Header("Player")]
    public PlayerUpgradeManager upgradeManager;
    public PlayerAttack playerAttack;
    public PlayerHealth playerHealth;
    public ResourceManager resourceManager;

    void Start()
    {
        // ×Ô¶¯Ñ°ÕÒ Player
        GameObject player =
            GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            if (upgradeManager == null)
                upgradeManager =
                    player.GetComponent<PlayerUpgradeManager>();

            if (playerAttack == null)
                playerAttack =
                    player.GetComponent<PlayerAttack>();

            if (playerHealth == null)
                playerHealth =
                    player.GetComponent<PlayerHealth>();

            if (resourceManager == null)
                resourceManager =
                    player.GetComponent<ResourceManager>();
        }

        UpdateUI();
    }

    void Update()
    {
        if (!gameObject.activeSelf)
            return;

        UpdateUI();
    }

    public void UpgradeAttack()
    {
        if (upgradeManager == null)
            return;

        bool success =
            upgradeManager.UpgradeAttack();

        if (success)
        {
            Debug.Log("Attack Upgrade Successful!");

            UpdateUI();
        }
    }

    public void UpgradeHealth()
    {
        if (upgradeManager == null)
            return;

        bool success =
            upgradeManager.UpgradeHealth();

        if (success)
        {
            Debug.Log("Health Upgrade Successful!");

            UpdateUI();
        }
    }

    void UpdateUI()
    {
        if (resourceManager != null &&
            goldText != null)
        {
            goldText.text =
                "Gold: " +
                resourceManager.gold;
        }

        if (playerAttack != null &&
            attackText != null)
        {
            attackText.text =
                "Attack: " +
                playerAttack.attackDamage +
                "\nCost: " +
                upgradeManager.attackUpgradeCost;
        }

        if (playerHealth != null &&
            healthText != null)
        {
            healthText.text =
                "Health: " +
                playerHealth.maxHealth +
                "\nCost: " +
                upgradeManager.healthUpgradeCost;
        }
    }

    public void CloseUI()
    {
        gameObject.SetActive(false);
    }


}

