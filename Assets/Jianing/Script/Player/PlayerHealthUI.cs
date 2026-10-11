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
        // 血条只用于显示，不拦截鼠标，也不能被拖动。
        CanvasGroup group = GetComponent<CanvasGroup>();
        group.interactable = false;
        group.blocksRaycasts = false;

        if (healthSlider == null)
        {
            healthSlider = GetComponentInChildren<Slider>(true);
        }

        if (healthSlider != null)
        {
            healthSlider.minValue = 0f;
            healthSlider.maxValue = 1f;
            healthSlider.wholeNumbers = false;
            healthSlider.interactable = false;
            healthSlider.direction = Slider.Direction.LeftToRight;

            Navigation navigation = healthSlider.navigation;
            navigation.mode = Navigation.Mode.None;
            healthSlider.navigation = navigation;
        }
    }

    private void Start()
    {
        // 没有手动指定时，尝试寻找场景中的玩家。
        if (playerHealth == null)
        {
            playerHealth = FindFirstObjectByType<PlayerHealth>();
        }

        if (playerHealth == null || healthSlider == null)
        {
            Debug.LogError(
                "PlayerHealthUI: Assign Player Health and Health Slider.",
                this
            );

            enabled = false;
        }
    }

    private void LateUpdate()
    {
        if (playerHealth == null || healthSlider == null)
            return;

        float maxHealth = Mathf.Max(
            0f,
            playerHealth.GetMaxHealth()
        );

        float currentHealth = Mathf.Clamp(
            playerHealth.GetCurrentHealth(),
            0f,
            maxHealth
        );

        // 血量没有变化时，不重复更新界面。
        if (Mathf.Approximately(currentHealth, displayedHealth) &&
            Mathf.Approximately(maxHealth, displayedMaxHealth))
        {
            return;
        }

        displayedHealth = currentHealth;
        displayedMaxHealth = maxHealth;

        float ratio = maxHealth > 0f
            ? currentHealth / maxHealth
            : 0f;

        healthSlider.SetValueWithoutNotify(ratio);

        if (healthText != null)
        {
            healthText.text =
                $"{Mathf.CeilToInt(currentHealth)} / " +
                $"{Mathf.CeilToInt(maxHealth)}";
        }
    }
}