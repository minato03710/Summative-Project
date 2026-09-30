using UnityEngine;
using UnityEngine.Serialization;

public class ResourceManager : MonoBehaviour
{
    [Header("Starting Resources")]
    [FormerlySerializedAs("wood")] public int energy = 0;
    [FormerlySerializedAs("stone")] public int battery = 0;
    public int food = 0;
    [FormerlySerializedAs("gold")] public int scrap = 30;

    public void AddResource(ResourcePickup.ResourceType type, int amount)
    {
        if (amount <= 0) return;

        switch (type)
        {
            case ResourcePickup.ResourceType.Energy:
                energy += amount;
                break;

            case ResourcePickup.ResourceType.Battery:
                battery += amount;
                break;

            case ResourcePickup.ResourceType.Food:
                food += amount;
                break;

            case ResourcePickup.ResourceType.Scrap:
                scrap += amount;
                break;
        }
    }

    public bool RemoveResource(ResourcePickup.ResourceType type, int amount)
    {
        if (amount < 0 || GetResource(type) < amount) return false;

        switch (type)
        {
            case ResourcePickup.ResourceType.Energy:
                energy -= amount;
                return true;

            case ResourcePickup.ResourceType.Battery:
                battery -= amount;
                return true;

            case ResourcePickup.ResourceType.Food:
                food -= amount;
                return true;

            case ResourcePickup.ResourceType.Scrap:
                scrap -= amount;
                return true;

            default:
                return false;
        }
    }

    public int GetResource(ResourcePickup.ResourceType type)
    {
        switch (type)
        {
            case ResourcePickup.ResourceType.Energy:
                return energy;

            case ResourcePickup.ResourceType.Battery:
                return battery;

            case ResourcePickup.ResourceType.Food:
                return food;

            case ResourcePickup.ResourceType.Scrap:
                return scrap;

            default:
                return 0;
        }
    }
}