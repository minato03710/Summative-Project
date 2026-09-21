using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [Header("Next Scene")]
    public string nextSceneName;


private bool hasTriggered = false;


    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered)
            return;

        if (!other.CompareTag("Player"))
            return;

        hasTriggered = true;


        // Save Player
        SavePlayerData(
            other.gameObject
        );


        // Save Companion
        SaveCompanionData();


        // Load Next Scene
        SceneManager.LoadScene(
            nextSceneName
        );
    }


    void SavePlayerData(
        GameObject player
    )
    {
        ResourceManager resourceManager =
            player.GetComponent<ResourceManager>();

        PlayerAttack playerAttack =
            player.GetComponent<PlayerAttack>();

        PlayerHealth playerHealth =
            player.GetComponent<PlayerHealth>();

        PlayerMovement playerMovement =
            player.GetComponent<PlayerMovement>();

        PlayerDash playerDash =
            player.GetComponent<PlayerDash>();


        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.SavePlayerData(
                resourceManager,
                playerAttack,
                playerHealth,
                playerMovement,
                playerDash
            );
        }
    }


    void SaveCompanionData()
    {
        CompanionCombat companionCombat =
            FindFirstObjectByType<CompanionCombat>();


        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.SaveCompanionData(
                companionCombat
            );
        }
    }


}
