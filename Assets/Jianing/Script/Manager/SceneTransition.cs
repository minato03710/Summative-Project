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

        SavePlayerData(other.gameObject);

        SceneManager.LoadScene(nextSceneName);
    }

    void SavePlayerData(GameObject player)
    {
        ResourceManager resourceManager =
            player.GetComponent<ResourceManager>();

        PlayerAttack playerAttack =
            player.GetComponent<PlayerAttack>();

        PlayerHealth playerHealth =
            player.GetComponent<PlayerHealth>();

        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.SavePlayerData(
                resourceManager,
                playerAttack,
                playerHealth
            );
        }
    }


}





