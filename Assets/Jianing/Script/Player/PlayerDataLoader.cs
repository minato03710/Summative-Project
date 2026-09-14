using UnityEngine;

public class PlayerDataLoader : MonoBehaviour
{
    private ResourceManager resourceManager;
    private PlayerAttack playerAttack;
    private PlayerHealth playerHealth;

void Start()
    {
        resourceManager =
            GetComponent<ResourceManager>();

        playerAttack =
            GetComponent<PlayerAttack>();

        playerHealth =
            GetComponent<PlayerHealth>();

        LoadData();
    }

    void LoadData()
    {
        if (GameDataManager.Instance == null)
            return;

        GameDataManager.Instance.LoadPlayerData(
            resourceManager,
            playerAttack,
            playerHealth
        );
    }


}

