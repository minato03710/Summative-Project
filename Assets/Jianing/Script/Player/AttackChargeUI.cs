using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AttackChargeUI : MonoBehaviour
{
    [Header("Player")]
    public PlayerAttack playerAttack;

    [Header("Font")]
    public TMP_FontAsset font;

    [Header("Offset From Left Center")]
    public Vector2 position = new Vector2(24f, 0f);

    [Header("Colors")]
    public Color chargingColor =
        new Color(0.2f, 0.75f, 1f, 1f);

    public Color readyColor =
        new Color(1f, 0.75f, 0.15f, 1f);

    private GameObject canvasObject;

    private RectTransform panel;
    private RectTransform fillRect;

    private Image fillImage;
    private TMP_Text progressText;

    private void Awake()
    {
        BuildUI();
    }

    private void LateUpdate()
    {
        if (playerAttack == null)
        {
            GameObject playerObject =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                playerAttack =
                    playerObject.GetComponent<PlayerAttack>();
            }
        }

        bool visible =
            playerAttack != null &&
            playerAttack.isActiveAndEnabled &&
            playerAttack.IsCharging &&
            Time.timeScale > 0f;

        panel.gameObject.SetActive(visible);

        if (!visible)
            return;

        panel.anchoredPosition = position;

        float progress = playerAttack.ChargeProgress;

        // 从底部向上填充，不依赖额外的 Sprite。
        fillRect.anchorMax = new Vector2(1f, progress);

        fillImage.color =
            playerAttack.IsFullyCharged
                ? readyColor
                : chargingColor;

        progressText.text =
            playerAttack.IsFullyCharged
                ? "READY\nRelease LMB"
                : Mathf.RoundToInt(progress * 100f) + "%";
    }

    private void BuildUI()
    {
        canvasObject = new GameObject(
            "Attack Charge Canvas",
            typeof(RectTransform),
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(CanvasGroup)
        );

        canvasObject.transform.SetParent(transform, false);

        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 70;

        CanvasScaler scaler =
            canvasObject.GetComponent<CanvasScaler>();

        scaler.uiScaleMode =
            CanvasScaler.ScaleMode.ScaleWithScreenSize;

        scaler.referenceResolution =
            new Vector2(1280f, 720f);

        scaler.matchWidthOrHeight = 0.5f;

        // 蓄力条不会挡住攻击或其他 UI 的鼠标输入。
        CanvasGroup group =
            canvasObject.GetComponent<CanvasGroup>();

        group.interactable = false;
        group.blocksRaycasts = false;

        panel = MakeRect(
            "Charge Panel",
            canvasObject.transform,
            new Vector2(110f, 260f)
        );

        panel.anchorMin = new Vector2(0f, 0.5f);
        panel.anchorMax = new Vector2(0f, 0.5f);
        panel.pivot = new Vector2(0f, 0.5f);
        panel.anchoredPosition = position;

        RectTransform background = MakeRect(
            "Bar Background",
            panel,
            new Vector2(28f, 170f)
        );

        Image backgroundImage =
            background.gameObject.AddComponent<Image>();

        backgroundImage.color =
            new Color(0.08f, 0.1f, 0.14f, 0.95f);

        backgroundImage.raycastTarget = false;

        RectTransform inner = MakeRect(
            "Inner",
            background,
            Vector2.zero
        );

        inner.anchorMin = Vector2.zero;
        inner.anchorMax = Vector2.one;

        inner.offsetMin = new Vector2(3f, 3f);
        inner.offsetMax = new Vector2(-3f, -3f);

        fillRect = MakeRect(
            "Fill",
            inner,
            Vector2.zero
        );

        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = new Vector2(1f, 0f);
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;

        fillImage = fillRect.gameObject.AddComponent<Image>();
        fillImage.color = chargingColor;
        fillImage.raycastTarget = false;

        MakeText(
            "Title",
            panel,
            new Vector2(0f, 110f),
            "CHARGE"
        );

        progressText = MakeText(
            "Progress",
            panel,
            new Vector2(0f, -110f),
            "0%"
        );

        panel.gameObject.SetActive(false);
    }

    private RectTransform MakeRect(
        string objectName,
        Transform parent,
        Vector2 size
    )
    {
        GameObject element = new GameObject(
            objectName,
            typeof(RectTransform),
            typeof(CanvasRenderer)
        );

        element.transform.SetParent(parent, false);

        RectTransform rect =
            element.GetComponent<RectTransform>();

        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);

        rect.sizeDelta = size;

        return rect;
    }

    private TMP_Text MakeText(
        string objectName,
        Transform parent,
        Vector2 location,
        string content
    )
    {
        RectTransform rect = MakeRect(
            objectName,
            parent,
            new Vector2(110f, 50f)
        );

        rect.anchoredPosition = location;

        TMP_Text text =
            rect.gameObject.AddComponent<TextMeshProUGUI>();

        text.font =
            font != null
                ? font
                : TMP_Settings.defaultFontAsset;

        text.text = content;
        text.fontSize = 19f;
        text.enableAutoSizing = true;
        text.fontSizeMin = 10f;
        text.fontSizeMax = 19f;

        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;
        text.raycastTarget = false;

        return text;
    }

    private void OnDisable()
    {
        if (panel != null)
        {
            panel.gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (canvasObject != null)
        {
            Destroy(canvasObject);
        }
    }
}