using UnityEngine;
using TMPro;

public class ResourceUI : MonoBehaviour
{
    [Header("Resource Text")]
    public TMP_Text woodText;
    public TMP_Text stoneText;
    public TMP_Text foodText;
    public TMP_Text goldText;

    private ResourceManager resourceManager;

    void Start()
    {
        GameObject player =
            GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            resourceManager =
                player.GetComponent<ResourceManager>();
        }

        UpdateUI();
    }

    void Update()
    {
        if (resourceManager == null)
            return;

        UpdateUI();
    }

    void UpdateUI()
    {
        woodText.text =
            "Wood: " + resourceManager.wood;

        stoneText.text =
            "Stone: " + resourceManager.stone;

        foodText.text =
            "Food: " + resourceManager.food;

        goldText.text =
            "Gold: " + resourceManager.gold;
    }
}


