using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    [Header("Starting Resources")]
    public int wood = 0;
    public int stone = 0;
    public int food = 0;
    public int gold = 30;

    public void AddResource(
        ResourcePickup.ResourceType type,
        int amount
    )
    {
        switch (type)
        {
            case ResourcePickup.ResourceType.Wood:
                wood += amount;
                break;

            case ResourcePickup.ResourceType.Stone:
                stone += amount;
                break;

            case ResourcePickup.ResourceType.Food:
                food += amount;
                break;

            case ResourcePickup.ResourceType.Gold:
                gold += amount;
                break;
        }

        Debug.Log(
            "Resources | " +
            "Wood: " + wood +
            " | Stone: " + stone +
            " | Food: " + food +
            " | Gold: " + gold
        );
    }

    public bool RemoveResource(
        ResourcePickup.ResourceType type,
        int amount
    )
    {
        switch (type)
        {
            case ResourcePickup.ResourceType.Wood:

                if (wood < amount)
                    return false;

                wood -= amount;
                return true;


            case ResourcePickup.ResourceType.Stone:

                if (stone < amount)
                    return false;

                stone -= amount;
                return true;


            case ResourcePickup.ResourceType.Food:

                if (food < amount)
                    return false;

                food -= amount;
                return true;


            case ResourcePickup.ResourceType.Gold:

                if (gold < amount)
                    return false;

                gold -= amount;
                return true;
        }

        return false;
    }

    public int GetResource(
        ResourcePickup.ResourceType type
    )
    {
        switch (type)
        {
            case ResourcePickup.ResourceType.Wood:
                return wood;

            case ResourcePickup.ResourceType.Stone:
                return stone;

            case ResourcePickup.ResourceType.Food:
                return food;

            case ResourcePickup.ResourceType.Gold:
                return gold;
        }

        return 0;
    }
}

