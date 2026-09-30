using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopUIManager : MonoBehaviour
{
    [Header("Stone Glove")]
    public TMP_Text stoneGloveText;
    public Button stoneGloveButton;

    [Header("Unarmed / Optional UI")]
    public Button unarmedButton;
    public TMP_Text resourceText;
    public TMP_Text messageText;

    [Header("Player")]
    public PlayerWeapon playerWeapon;

    private void OnEnable()
    {
        FindPlayer();
        SetMessage("");
        RefreshUI();
    }

    private void FindPlayer()
    {
        if (playerWeapon != null) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
            playerWeapon = player.GetComponent<PlayerWeapon>();
    }

    public void BuyStoneGlove()
    {
        FindPlayer();

        if (playerWeapon == null)
        {
            SetMessage("PlayerWeapon is missing on the Player.");
            return;
        }

        bool alreadyOwned = playerWeapon.HasStoneGlove();
        bool success = playerWeapon.BuyStoneGlove();

        SetMessage(success
            ? (alreadyOwned
                ? "Stone Glove equipped."
                : "Purchased and equipped Stone Glove.")
            : "Not enough Scrap, or invalid weapon setup.");

        RefreshUI();
    }

    public void EquipStoneGlove()
    {
        FindPlayer();

        if (playerWeapon == null || !playerWeapon.HasStoneGlove())
            return;

        playerWeapon.EquipStoneGlove();
        SetMessage("Stone Glove equipped.");
        RefreshUI();
    }

    public void EquipUnarmed()
    {
        FindPlayer();

        if (playerWeapon == null) return;

        playerWeapon.UnequipStoneGlove();
        SetMessage("Unarmed equipped.");
        RefreshUI();
    }

    public void RefreshUI()
    {
        FindPlayer();

        bool ready = playerWeapon != null;
        bool owned = ready && playerWeapon.HasStoneGlove();
        bool equipped = ready && playerWeapon.IsStoneGloveEquipped();

        if (stoneGloveText != null)
        {
            stoneGloveText.text = !ready
                ? "PlayerWeapon is missing."
                : "Stone Glove\nDamage: x" +
                  playerWeapon.stoneGloveDamageMultiplier.ToString("0.##") +
                  "\nAttack interval: " +
                  playerWeapon.stoneGloveAttackCooldown.ToString("0.##") +
                  " s\n" +
                  (equipped
                      ? "EQUIPPED"
                      : owned
                          ? "OWNED"
                          : "Cost: " + playerWeapon.stoneGloveScrapCost + " Scrap");
        }

        if (stoneGloveButton != null)
        {
            stoneGloveButton.interactable = ready && !equipped;

            SetButtonText(
                stoneGloveButton,
                equipped ? "EQUIPPED" : owned ? "EQUIP" : "BUY");
        }

        if (unarmedButton != null)
        {
            unarmedButton.interactable = ready && equipped;

            SetButtonText(
                unarmedButton,
                equipped ? "EQUIP UNARMED" : "UNARMED EQUIPPED");
        }

        if (resourceText != null)
        {
            resourceText.text = ready
                ? "Scrap: " + playerWeapon.ScrapCount
                : "Scrap: --";
        }
    }

    private void SetMessage(string message)
    {
        if (messageText != null)
            messageText.text = message;
    }

    private static void SetButtonText(Button button, string label)
    {
        TMP_Text text = button.GetComponentInChildren<TMP_Text>(true);

        if (text != null)
            text.text = label;
    }
}