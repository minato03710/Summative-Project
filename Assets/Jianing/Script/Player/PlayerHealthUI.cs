using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(CanvasGroup))]
public class PlayerHealthUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TMP_Text healthText;

    private float displayedHealth = -1f;
    private float displayedMaxHealth = -1f;

    private void Awake()
    {
        if (healthSlider == null)
            healthSlider = GetComponentInChildren<Slider>(true);

        if (healthSlider == null)
        {
            Debug.LogError(
                "PlayerHealthUI: Assign Health Slider.",
                this
            );

            enabled = false;
            return;
        }

        // Display only: no clicking, dragging or selection.
        healthSlider.transition = Selectable.Transition.None;
        healthSlider.interactable = false;
        healthSlider.minValue = 0f;
        healthSlider.maxValue = 1f;
        healthSlider.wholeNumbers = false;
        healthSlider.direction = Slider.Direction.LeftToRight;

        Navigation navigation = healthSlider.navigation;
        navigation.mode = Navigation.Mode.None;
        healthSlider.navigation = navigation;

        CanvasGroup group = GetComponent<CanvasGroup>();
        group.interactable = false;
        group.blocksRaycasts = false;
    }

    private void OnEnable()
    {
        ResetDisplayedValues();
    }

    private void LateUpdate()
    {
        FindPlayer();

        if (playerHealth == null || healthSlider == null)
            return;

        float maxHealth = Mathf.Max(
            1f,
            playerHealth.GetMaxHealth()
        );

        float currentHealth = Mathf.Clamp(
            playerHealth.GetCurrentHealth(),
            0f,
            maxHealth
        );

        if (Mathf.Approximately(currentHealth, displayedHealth) &&
            Mathf.Approximately(maxHealth, displayedMaxHealth))
        {
            return;
        }

        displayedHealth = currentHealth;
        displayedMaxHealth = maxHealth;

        healthSlider.SetValueWithoutNotify(
            currentHealth / maxHealth
        );

        if (healthText != null)
        {
            healthText.text =
                $"{Mathf.CeilToInt(currentHealth)} / " +
                $"{Mathf.CeilToInt(maxHealth)}";
        }
    }

    private void FindPlayer()
    {
        // Prefer the player preserved between scenes.
        // Replace references to a duplicate scene player.
        if (PlayerPersistence.Instance != null)
        {
            PlayerHealth persistentHealth =
                PlayerPersistence.Instance
                    .GetComponent<PlayerHealth>();

            if (persistentHealth != null)
            {
                if (playerHealth != persistentHealth)
                {
                    playerHealth = persistentHealth;
                    ResetDisplayedValues();
                }

                return;
            }
        }

        // Support scenes without a persistent player.
        // Retry later if no player exists yet.
        if (playerHealth == null)
        {
            playerHealth =
                FindFirstObjectByType<PlayerHealth>();

            ResetDisplayedValues();
        }
    }

    private void ResetDisplayedValues()
    {
        displayedHealth = -1f;
        displayedMaxHealth = -1f;
    }
}