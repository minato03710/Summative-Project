using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopUIManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text stoneGloveText;
    public Button stoneGloveButton;


[Header("Player")]
    public PlayerWeapon playerWeapon;

    void Start()
    {
        FindPlayer();

        UpdateUI();
    }

    void FindPlayer()
    {
        GameObject player =
            GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogWarning(
                "ShopUIManager: Player not found!"
            );

            return;
        }

        playerWeapon =
            player.GetComponent<PlayerWeapon>();
    }

    public void BuyStoneGlove()
    {
        if (playerWeapon == null)
        {
            FindPlayer();
        }

        if (playerWeapon == null)
            return;

        bool success =
            playerWeapon.BuyStoneGlove();

        if (success)
        {
            UpdateUI();
        }
    }

    public void EquipStoneGlove()
    {
        if (playerWeapon == null)
        {
            FindPlayer();
        }

        if (playerWeapon == null)
            return;

        if (!playerWeapon.HasStoneGlove())
            return;

        playerWeapon.EquipStoneGlove();

        UpdateUI();
    }

    void UpdateUI()
    {
        if (playerWeapon == null)
            return;

        if (stoneGloveText != null)
        {
            if (playerWeapon.HasStoneGlove())
            {
                if (playerWeapon.IsStoneGloveEquipped())
                {
                    stoneGloveText.text =
                        "Stone Glove\n\nEQUIPPED";
                }
                else
                {
                    stoneGloveText.text =
                        "Stone Glove\n\nOWNED";
                }
            }
            else
            {
                stoneGloveText.text =
                    "Stone Glove\n\nCost: 5 Stone";
            }
        }

        if (stoneGloveButton != null)
        {
            TMP_Text buttonText =
                stoneGloveButton.GetComponentInChildren<TMP_Text>();

            if (buttonText != null)
            {
                if (!playerWeapon.HasStoneGlove())
                {
                    buttonText.text = "BUY";
                }
                else if (
                    playerWeapon.IsStoneGloveEquipped())
                {
                    buttonText.text = "EQUIPPED";
                }
                else
                {
                    buttonText.text = "EQUIP";
                }
            }
        }
    }


}
