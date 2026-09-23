using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [Header("Stone Glove")]
    public int stoneGloveCost = 5;
    public float stoneGloveDamageMultiplier = 1.5f;
    public float stoneGloveAttackCooldown = 1.2f;


private PlayerAttack playerAttack;
    private ResourceManager resourceManager;

    private bool hasStoneGlove = false;
    private bool stoneGloveEquipped = false;

    private void Start()
    {
        playerAttack =
            GetComponent<PlayerAttack>();

        resourceManager =
            GetComponent<ResourceManager>();
    }

    public bool BuyStoneGlove()
    {
        if (hasStoneGlove)
        {
            Debug.Log("Stone Glove already purchased!");

            EquipStoneGlove();

            return true;
        }

        if (resourceManager == null)
        {
            Debug.LogWarning(
                "PlayerWeapon: ResourceManager not found!"
            );

            return false;
        }

        if (resourceManager.stone < stoneGloveCost)
        {
            Debug.Log("Not enough Stone!");
            return false;
        }

        resourceManager.RemoveResource(
            ResourcePickup.ResourceType.Stone,
            stoneGloveCost
        );

        hasStoneGlove = true;

        Debug.Log("Stone Glove purchased!");

        EquipStoneGlove();

        return true;
    }

    public void EquipStoneGlove()
    {
        if (!hasStoneGlove)
        {
            Debug.Log(
                "You have not purchased Stone Glove!"
            );

            return;
        }

        if (playerAttack == null)
        {
            playerAttack =
                GetComponent<PlayerAttack>();
        }

        if (playerAttack == null)
            return;

        playerAttack.ApplyWeaponStats(
            stoneGloveDamageMultiplier,
            stoneGloveAttackCooldown
        );

        stoneGloveEquipped = true;

        Debug.Log(
            "Stone Glove Equipped!"
        );
    }

    public void UnequipStoneGlove()
    {
        if (playerAttack == null)
        {
            playerAttack =
                GetComponent<PlayerAttack>();
        }

        if (playerAttack == null)
            return;

        playerAttack.RemoveWeaponStats();

        stoneGloveEquipped = false;

        Debug.Log(
            "Stone Glove Unequipped!"
        );
    }

    public bool HasStoneGlove()
    {
        return hasStoneGlove;
    }

    public bool IsStoneGloveEquipped()
    {
        return stoneGloveEquipped;
    }


}
