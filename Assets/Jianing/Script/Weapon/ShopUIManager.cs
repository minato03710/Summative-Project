using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopUIManager : MonoBehaviour
{
    [Header("Stone Glove")]
    public TMP_Text stoneGloveText;
    public Button stoneGloveButton;

    [Header("Snipping Claws")]
    public TMP_Text snippingClawsText;
    public Button snippingClawsButton;

    [Header("Scorpion Tail")]
    public TMP_Text scorpionTailText;
    public Button scorpionTailButton;

    [Header("Sword(Fish)")]
    public TMP_Text swordFishText;
    public Button swordFishButton;

    [Header("Ranged Weapons")]
    public TMP_Text standardGunText;
    public TMP_Text energyMachineGunText;
    public TMP_Text batteryAcidText;

    public Button standardGunButton;
    public Button energyMachineGunButton;
    public Button batteryAcidButton;

    [Header("Other UI")]
    public Button unarmedButton;
    public TMP_Text resourceText;
    public TMP_Text messageText;

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

        GameObject player =
            GameObject.FindGameObjectWithTag("Player");

        if (player != null)
            playerWeapon = player.GetComponent<PlayerWeapon>();
    }

    public void BuyStoneGlove()
    {
        Buy(WeaponType.StoneGlove);
    }

    public void BuySnippingClaws()
    {
        Buy(WeaponType.SnippingClaws);
    }

    public void BuyScorpionTail()
    {
        Buy(WeaponType.ScorpionTail);
    }

    public void BuySwordFish()
    {
        Buy(WeaponType.SwordFish);
    }

    public void BuyStandardGun()
    {
        Buy(WeaponType.StandardGun);
    }

    public void BuyEnergyMachineGun()
    {
        Buy(WeaponType.EnergyMachineGun);
    }

    public void BuyBatteryAcid()
    {
        Buy(WeaponType.BatteryAcidProjectiles);
    }

    public void EquipStoneGlove()
    {
        EquipOnly(WeaponType.StoneGlove);
    }

    public void EquipUnarmed()
    {
        EquipOnly(WeaponType.Unarmed);
    }

    private void Buy(WeaponType type)
    {
        FindPlayer();

        if (playerWeapon == null)
        {
            SetMessage("PlayerWeapon is missing.");
            return;
        }

        bool owned = playerWeapon.IsOwned(type);
        bool success = playerWeapon.BuyOrEquip(type);

        SetMessage(
            success
                ? (owned ? "Equipped." : "Purchased and equipped.")
                : "Not enough resources, or invalid weapon setup.");

        RefreshUI();
    }

    private void EquipOnly(WeaponType type)
    {
        FindPlayer();

        if (playerWeapon == null) return;

        SetMessage(
            playerWeapon.Equip(type)
                ? "Equipped."
                : "Weapon is not owned.");

        RefreshUI();
    }

    public void RefreshUI()
    {
        FindPlayer();

        RefreshSlot(
            WeaponType.StoneGlove,
            stoneGloveText,
            stoneGloveButton);

        RefreshSlot(
            WeaponType.SnippingClaws,
            snippingClawsText,
            snippingClawsButton);

        RefreshSlot(
            WeaponType.ScorpionTail,
            scorpionTailText,
            scorpionTailButton);

        RefreshSlot(
            WeaponType.SwordFish,
            swordFishText,
            swordFishButton);

        RefreshSlot(
            WeaponType.StandardGun,
            standardGunText,
            standardGunButton);

        RefreshSlot(
            WeaponType.EnergyMachineGun,
            energyMachineGunText,
            energyMachineGunButton);

        RefreshSlot(
            WeaponType.BatteryAcidProjectiles,
            batteryAcidText,
            batteryAcidButton);

        bool unarmed =
            playerWeapon != null &&
            playerWeapon.EquippedWeapon == WeaponType.Unarmed;

        if (unarmedButton != null)
        {
            unarmedButton.interactable =
                playerWeapon != null && !unarmed;

            SetButtonText(
                unarmedButton,
                unarmed ? "UNARMED EQUIPPED" : "EQUIP UNARMED");
        }

        if (resourceText != null)
        {
            resourceText.text = playerWeapon == null
                ? "Player missing"
                : $"Scrap: {playerWeapon.ScrapCount}   " +
                  $"Battery: {playerWeapon.BatteryCount}   " +
                  $"Energy: {playerWeapon.EnergyCount}";
        }
    }

    private void RefreshSlot(
        WeaponType type,
        TMP_Text text,
        Button button)
    {
        WeaponData data = playerWeapon != null
            ? playerWeapon.GetWeaponData(type)
            : null;

        bool owned =
            playerWeapon != null &&
            playerWeapon.IsOwned(type);

        bool equipped =
            playerWeapon != null &&
            playerWeapon.EquippedWeapon == type;

        if (button != null)
        {
            button.interactable = data != null && !equipped;

            SetButtonText(
                button,
                equipped ? "EQUIPPED" : owned ? "EQUIP" : "BUY");
        }

        if (text == null) return;

        if (data == null)
        {
            text.text = "Weapon data missing.";
            return;
        }

        string cost = "";

        if (data.scrapCost > 0)
            cost = $"{data.scrapCost} Scrap";

        if (data.batteryCost > 0)
        {
            if (cost.Length > 0)
                cost += " / ";

            cost += $"{data.batteryCost} Battery";
        }

        if (data.energyCost > 0)
        {
            if (cost.Length > 0)
                cost += " / ";

            cost += $"{data.energyCost} Energy";
        }

        if (cost.Length == 0)
            cost = "Free";

        text.text = $"{data.weaponName}\nCost: {cost}";
    }

    private void SetMessage(string message)
    {
        if (messageText != null)
            messageText.text = message;
    }

    private static void SetButtonText(Button button, string label)
    {
        TMP_Text text =
            button.GetComponentInChildren<TMP_Text>(true);

        if (text != null)
            text.text = label;
    }
}