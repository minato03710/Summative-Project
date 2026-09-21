using UnityEngine;

public class PlayerDataLoader : MonoBehaviour
{
    private ResourceManager resourceManager;
    private PlayerAttack playerAttack;
    private PlayerHealth playerHealth;
    private PlayerMovement playerMovement;
    private PlayerDash playerDash;


void Start()
    {
        resourceManager =
            GetComponent<ResourceManager>();

        playerAttack =
            GetComponent<PlayerAttack>();

        playerHealth =
            GetComponent<PlayerHealth>();

        playerMovement =
            GetComponent<PlayerMovement>();

        playerDash =
            GetComponent<PlayerDash>();

        LoadData();
    }

    void LoadData()
    {
        if (GameDataManager.Instance == null)
            return;

        GameDataManager.Instance.LoadPlayerData(
            resourceManager,
            playerAttack,
            playerHealth,
            playerMovement,
            playerDash
        );
    }


}
