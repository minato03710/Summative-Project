using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using TMPro;

[DefaultExecutionOrder(-200)]
public class CompanionWheelMenu : MonoBehaviour
{
    public enum CommandType
    {
        Heal,
        Boost,
        Sit,
        FollowAndAttack,
        Custom
    }

    [Serializable]
    public class WheelOption
    {
        public string label;
        public CommandType command;
        public UnityEvent onSelected = new UnityEvent();
    }

    [Header("Companion")]
    public CompanionCommandController companion;

    [Header("Options")]
    public List<WheelOption> options = new List<WheelOption>
    {
        new WheelOption
        {
            label = "HEAL",
            command = CommandType.Heal
        },

        new WheelOption
        {
            label = "BOOST",
            command = CommandType.Boost
        },

        new WheelOption
        {
            label = "STAY",
            command = CommandType.Sit
        },

        new WheelOption
        {
            label = "FOLLOW + ATTACK",
            command = CommandType.FollowAndAttack
        }
    };

    [Header("Appearance")]
    public TMP_FontAsset font;

    [Min(120f)]
    public float outerRadius = 230f;

    [Min(0f)]
    public float innerRadius = 65f;

    [Range(0f, 15f)]
    public float sliceGap = 4f;

    public Color sliceColor =
        new Color(0.10f, 0.17f, 0.24f, 0.98f);

    public Color highlightColor =
        new Color(0.18f, 0.65f, 0.65f, 1f);

    [Header("Optional Extra Scripts To Pause")]
    public Behaviour[] additionalPausedScripts;

    private GameObject wheelCanvas;

    private TMP_Text titleText;
    private TMP_Text messageText;

    private readonly List<Button> buttons =
        new List<Button>();

    private readonly List<TMP_Text> labels =
        new List<TMP_Text>();

    private readonly List<Behaviour> suspended =
        new List<Behaviour>();

    private bool isOpen;
    private float previousTimeScale;

    private CursorLockMode previousCursorLock;
    private bool previousCursorVisible;

    public bool IsOpen => isOpen;

    private void Awake()
    {
        BuildWheel();
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;

        if (isOpen)
        {
            if (keyboard == null ||
                !keyboard.qKey.isPressed ||
                companion == null ||
                !companion.isActiveAndEnabled)
            {
                CloseWheel();
                return;
            }

            RefreshButtons();
            return;
        }

        if (keyboard != null &&
            keyboard.qKey.wasPressedThisFrame)
        {
            OpenWheel();
        }
    }

    public void OpenWheel()
    {
        // 不覆盖其他界面已经持有的暂停状态。
        if (isOpen || Time.timeScale <= 0f)
            return;

        if (companion == null)
        {
            companion =
                FindFirstObjectByType<CompanionCommandController>();
        }

        if (companion == null ||
            !companion.isActiveAndEnabled)
        {
            Debug.LogWarning(
                "Assign an active CompanionCommandController.",
                this
            );

            return;
        }

        EnsureEventSystem();

        previousTimeScale = Time.timeScale;
        previousCursorLock = Cursor.lockState;
        previousCursorVisible = Cursor.visible;

        isOpen = true;

        SuspendGameplay();

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        wheelCanvas.SetActive(true);

        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        messageText.text =
            "Hold Q and click a command. Release Q to resume.";

        RefreshButtons();
    }

    public void CloseWheel()
    {
        if (!isOpen)
            return;

        isOpen = false;

        if (wheelCanvas != null)
        {
            wheelCanvas.SetActive(false);
        }

        Time.timeScale = previousTimeScale;

        Cursor.lockState = previousCursorLock;
        Cursor.visible = previousCursorVisible;

        foreach (Behaviour behaviour in suspended)
        {
            if (behaviour == null)
                continue;

            // 玩家死亡后，不重新启用移动组件。
            if (behaviour is PlayerMovement ||
                behaviour is PlayerDash)
            {
                PlayerHealth health =
                    behaviour.GetComponent<PlayerHealth>();

                if (health != null && health.IsDead())
                    continue;
            }

            behaviour.enabled = true;
        }

        suspended.Clear();

        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    private void SuspendGameplay()
    {
        // 部分旧脚本在 timeScale = 0 时仍可能处理输入或攻击。
        MonoBehaviour[] behaviours =
            FindObjectsByType<MonoBehaviour>(
                FindObjectsSortMode.None
            );

        foreach (MonoBehaviour behaviour in behaviours)
        {
            if (behaviour is EnemyAI ||
                behaviour is PlayerMovement ||
                behaviour is PlayerDash ||
                behaviour is UpgradeConsole)
            {
                Suspend(behaviour);
            }
        }

        if (additionalPausedScripts == null)
            return;

        foreach (Behaviour behaviour in additionalPausedScripts)
        {
            Suspend(behaviour);
        }
    }

    private void Suspend(Behaviour behaviour)
    {
        if (behaviour == null ||
            behaviour == this ||
            !behaviour.enabled ||
            suspended.Contains(behaviour))
        {
            return;
        }

        suspended.Add(behaviour);
        behaviour.enabled = false;
    }

    private void SelectCommand(int index)
    {
        if (!isOpen ||
            Keyboard.current == null ||
            !Keyboard.current.qKey.isPressed ||
            companion == null ||
            index < 0 ||
            index >= options.Count)
        {
            return;
        }

        WheelOption option = options[index];

        if (option == null || !CanSelect(option.command))
            return;

        switch (option.command)
        {
            case CommandType.Heal:
                companion.HealPlayer();
                break;

            case CommandType.Boost:
                companion.BoostCompanion();
                break;

            case CommandType.Sit:
                companion.Sit();
                break;

            case CommandType.FollowAndAttack:
                companion.FollowAndAttack();
                break;
        }

        if (option.onSelected != null)
        {
            option.onSelected.Invoke();
        }

        if (messageText != null)
        {
            messageText.text =
                option.command == CommandType.Custom
                    ? option.label
                    : companion.LastResult;
        }

        RefreshButtons();

        // 点击指令后保持打开，松开 Q 才关闭。
    }

    private bool CanSelect(CommandType command)
    {
        if (companion == null)
            return false;

        switch (command)
        {
            case CommandType.Heal:
                return companion.CanHeal;

            case CommandType.Boost:
                return companion.CanBoost;

            case CommandType.Sit:
                return !companion.IsSitting;

            case CommandType.FollowAndAttack:
                return companion.IsSitting;

            default:
                return true;
        }
    }

    private void RefreshButtons()
    {
        if (companion == null)
            return;

        titleText.text =
            companion.IsSitting ? "STAYING" : "FOLLOWING";

        for (int i = 0; i < buttons.Count; i++)
        {
            WheelOption option = options[i];

            buttons[i].interactable =
                option != null && CanSelect(option.command);

            if (option == null)
                continue;

            string detail = "";

            if (option.command == CommandType.Heal)
            {
                detail =
                    companion.HealCooldownRemaining > 0f
                        ? companion.HealCooldownRemaining
                            .ToString("0.0") + "s cooldown"
                        : "+" + companion.healAmount
                            .ToString("0.#") + " HP";
            }
            else if (option.command == CommandType.Boost)
            {
                if (companion.BoostCooldownRemaining > 0f)
                {
                    detail =
                        companion.BoostCooldownRemaining
                            .ToString("0.0") + "s cooldown";
                }
                else
                {
                    detail =
                        "x" +
                        companion.boostDamageMultiplier.ToString("0.#") +
                        " damage\nx" +
                        companion.boostSpeedMultiplier.ToString("0.#") +
                        " speed / " +
                        companion.boostDuration.ToString("0.#") +
                        "s";
                }
            }

            labels[i].text =
                option.label +
                (detail.Length > 0 ? "\n" + detail : "");
        }
    }

    private void BuildWheel()
    {
        wheelCanvas = new GameObject(
            "Companion Wheel Canvas",
            typeof(RectTransform),
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster)
        );

        wheelCanvas.transform.SetParent(transform, false);

        Canvas canvas = wheelCanvas.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;

        CanvasScaler scaler =
            wheelCanvas.GetComponent<CanvasScaler>();

        scaler.uiScaleMode =
            CanvasScaler.ScaleMode.ScaleWithScreenSize;

        scaler.referenceResolution =
            new Vector2(1280f, 720f);

        scaler.matchWidthOrHeight = 0.5f;

        // 全屏背景，阻止 UI 点击穿透。
        GameObject backdrop = new GameObject(
            "Block World Clicks",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Image)
        );

        backdrop.transform.SetParent(
            wheelCanvas.transform,
            false
        );

        RectTransform backdropRect =
            backdrop.GetComponent<RectTransform>();

        backdropRect.anchorMin = Vector2.zero;
        backdropRect.anchorMax = Vector2.one;
        backdropRect.offsetMin = Vector2.zero;
        backdropRect.offsetMax = Vector2.zero;

        backdrop.GetComponent<Image>().color =
            new Color(0f, 0f, 0f, 0.45f);

        float outer = Mathf.Max(120f, outerRadius);

        float inner = Mathf.Clamp(
            innerRadius,
            0f,
            outer - 50f
        );

        int count = options == null ? 0 : options.Count;

        for (int i = 0; i < count; i++)
        {
            // CanvasRenderer 必须在添加自定义 Graphic 前存在。
            GameObject slice = new GameObject(
                "Command " + i,
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(RadialSliceGraphic),
                typeof(Button)
            );

            slice.transform.SetParent(
                wheelCanvas.transform,
                false
            );

            RectTransform rect =
                slice.GetComponent<RectTransform>();

            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);

            rect.sizeDelta = new Vector2(
                outer * 2f,
                outer * 2f
            );

            float angle =
                90f - i * (360f / count);

            RadialSliceGraphic graphic =
                slice.GetComponent<RadialSliceGraphic>();

            graphic.color = Color.white;

            graphic.Configure(
                inner,
                outer,
                angle,
                Mathf.Max(1f, 360f / count - sliceGap)
            );

            Button button = slice.GetComponent<Button>();
            button.targetGraphic = graphic;

            ColorBlock colors = button.colors;

            colors.normalColor = sliceColor;
            colors.highlightedColor = highlightColor;
            colors.pressedColor = highlightColor * 0.8f;
            colors.selectedColor = sliceColor;

            colors.disabledColor =
                new Color(0.16f, 0.18f, 0.20f, 0.95f);

            colors.fadeDuration = 0.08f;

            button.colors = colors;

            Navigation navigation = button.navigation;
            navigation.mode = Navigation.Mode.None;
            button.navigation = navigation;

            int capturedIndex = i;

            button.onClick.AddListener(
                () => SelectCommand(capturedIndex)
            );

            buttons.Add(button);

            float labelRadius =
                (inner + outer) * 0.5f;

            Vector2 labelPosition =
                new Vector2(
                    Mathf.Cos(angle * Mathf.Deg2Rad),
                    Mathf.Sin(angle * Mathf.Deg2Rad)
                ) * labelRadius;

            TMP_Text label = CreateText(
                slice.transform,
                "Label",
                labelPosition,
                new Vector2(
                    count > 6 ? 90f : 140f,
                    90f
                ),
                19f
            );

            labels.Add(label);
        }

        titleText = CreateText(
            wheelCanvas.transform,
            "Mode",
            Vector2.zero,
            new Vector2(inner * 1.8f, 70f),
            16f
        );

        messageText = CreateText(
            wheelCanvas.transform,
            "Hint",
            new Vector2(0f, -outer - 48f),
            new Vector2(850f, 80f),
            21f
        );

        wheelCanvas.SetActive(false);
    }

    private TMP_Text CreateText(
        Transform parent,
        string objectName,
        Vector2 position,
        Vector2 size,
        float fontSize
    )
    {
        GameObject textObject = new GameObject(
            objectName,
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(TextMeshProUGUI)
        );

        textObject.transform.SetParent(parent, false);

        TMP_Text text =
            textObject.GetComponent<TMP_Text>();

        RectTransform rect = text.rectTransform;

        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);

        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        text.font =
            font != null
                ? font
                : TMP_Settings.defaultFontAsset;

        text.fontSize = fontSize;
        text.enableAutoSizing = true;
        text.fontSizeMin = 10f;
        text.fontSizeMax = fontSize;

        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;
        text.raycastTarget = false;

        return text;
    }

    private void EnsureEventSystem()
    {
        EventSystem existing =
            FindFirstObjectByType<EventSystem>();

        if (existing != null)
            return;

        GameObject eventSystem = new GameObject(
            "EventSystem",
            typeof(EventSystem),
            typeof(InputSystemUIInputModule)
        );

        eventSystem
            .GetComponent<InputSystemUIInputModule>()
            .AssignDefaultActions();
    }

    private void OnDisable()
    {
        CloseWheel();
    }

    private void OnDestroy()
    {
        CloseWheel();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            CloseWheel();
        }
    }
}