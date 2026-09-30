using UnityEngine;
using UnityEngine.Serialization;
using TMPro;

public class ResourceUI : MonoBehaviour
{
    [Header("Resource Text")]
    [FormerlySerializedAs("woodText")] public TMP_Text energyText;
    [FormerlySerializedAs("stoneText")] public TMP_Text batteryText;
    public TMP_Text foodText;
    [FormerlySerializedAs("goldText")] public TMP_Text scrapText;

    private ResourceManager resourceManager;

    private void Update()
    {
        if (resourceManager == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player != null)
                resourceManager = player.GetComponent<ResourceManager>();
        }

        if (resourceManager == null) return;

        if (energyText != null)
            energyText.text = "Energy: " + resourceManager.energy;

        if (batteryText != null)
            batteryText.text = "Battery: " + resourceManager.battery;

        if (foodText != null)
            foodText.text = "Food: " + resourceManager.food;

        if (scrapText != null)
            scrapText.text = "Scrap: " + resourceManager.scrap;
    }
}