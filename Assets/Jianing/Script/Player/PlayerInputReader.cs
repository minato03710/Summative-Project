using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-500)]
[DisallowMultipleComponent]
[RequireComponent(typeof(PlayerInput))]
public class PlayerInputReader : MonoBehaviour
{
    public static PlayerInputReader Instance { get; private set; }

    public PlayerInput Input { get; private set; }

    private PlayerHealth health;

    private InputAction attack;
    private InputAction jump;
    private InputAction dash;
    private InputAction interact;
    private InputAction wheel;

    private InputAction point;
    private InputAction navigate;
    private InputAction cancel;

    private bool ready;
    private bool waitForRelease;
    private bool hasStickAim;

    private int blockedThroughFrame = -1;

    private int attackFrame = -1;
    private int releaseFrame = -1;
    private int jumpFrame = -1;
    private int dashFrame = -1;
    private int interactFrame = -1;
    private int wheelFrame = -1;

    private Vector2 movement;
    private Vector3 lastStickAim;
    private Camera aimCamera;

    public bool UsingGamepad =>
        Input != null &&
        Input.currentControlScheme == "Gamepad";

    public bool GameplayAllowed =>
        ready &&
        isActiveAndEnabled &&
        Input != null &&
        Input.isActiveAndEnabled &&
        Input.inputIsActive &&
        (health == null || !health.IsDead()) &&
        Time.timeScale > 0f &&
        !GamepadMenu.AnyOpen &&
        !waitForRelease &&
        Time.frameCount > blockedThroughFrame;

    public Vector2 Move =>
        Vector2.ClampMagnitude(movement, 1f);

    public Vector2 MenuDirection => navigate != null
        ? navigate.ReadValue<Vector2>()
        : Vector2.zero;

    public bool AttackHeld =>
        attack != null && attack.IsPressed();

    public bool WheelHeld =>
        wheel != null && wheel.IsPressed();

    public bool AttackPressed =>
        attackFrame == Time.frameCount;

    public bool AttackReleased =>
        releaseFrame == Time.frameCount;

    public bool JumpPressed =>
        jumpFrame == Time.frameCount;

    public bool DashPressed =>
        dashFrame == Time.frameCount;

    public bool InteractPressed =>
        interactFrame == Time.frameCount;

    public bool WheelPressed =>
        wheelFrame == Time.frameCount;

    public bool CancelPressed =>
        cancel != null && cancel.WasPressedThisFrame();

    private void Awake()
    {
        Input = GetComponent<PlayerInput>();
        health = GetComponent<PlayerHealth>();

        // Keep the input reader belonging to the existing player.
        if (Instance != null && Instance != this)
        {
            enabled = false;
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        if (Input.actions == null)
        {
            Debug.LogError(
                "Assign InputSystem_Actions to PlayerInput.",
                this
            );

            enabled = false;
            return;
        }

        try
        {
            // 使用现有 PlayerInput 的 Actions。
            attack = Input.actions.FindAction("Player/Attack", true);
            jump = Input.actions.FindAction("Player/Jump", true);
            dash = Input.actions.FindAction("Player/Sprint", true);
            interact = Input.actions.FindAction("Player/Interact", true);
            wheel = Input.actions.FindAction("Player/CompanionWheel", true);

            point = Input.actions.FindAction("UI/Point", true);
            navigate = Input.actions.FindAction("UI/Navigate", true);
            cancel = Input.actions.FindAction("UI/Cancel", true);
        }
        catch (ArgumentException error)
        {
            Debug.LogError(
                "PlayerInput action missing: " + error.Message,
                this
            );

            enabled = false;
            return;
        }

        ready = true;

        GamepadMenu.EnsureEventSystem();
    }

    public void ReadMove(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>();
    }

    public void ReadJump(InputAction.CallbackContext context)
    {
        if (context.performed)
            jumpFrame = Time.frameCount;
    }

    public void ReadDash(InputAction.CallbackContext context)
    {
        if (context.performed)
            dashFrame = Time.frameCount;
    }

    public void ReadAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
            attackFrame = Time.frameCount;

        if (context.canceled)
            releaseFrame = Time.frameCount;
    }

    public void ReadInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
            interactFrame = Time.frameCount;
    }

    public void ReadWheel(InputAction.CallbackContext context)
    {
        if (context.performed)
            wheelFrame = Time.frameCount;
    }

    public void ReadLook(InputAction.CallbackContext context)
    {
        if (!context.performed ||
            !(context.control.device is Gamepad))
        {
            return;
        }

        Vector2 stick = context.ReadValue<Vector2>();

        if (stick.sqrMagnitude <= 0.04f)
            return;

        lastStickAim =
            new Vector3(stick.x, 0f, stick.y).normalized;

        hasStickAim = true;
    }

    private void Update()
    {
        bool menuMode =
            Time.timeScale <= 0f || GamepadMenu.AnyOpen;

        if (EventSystem.current != null)
            EventSystem.current.sendNavigationEvents = menuMode;

        if (menuMode)
        {
            SuppressGameplay();
            return;
        }

        if (Time.frameCount > blockedThroughFrame &&
            !AnyButtonHeld())
        {
            waitForRelease = false;
        }
    }

    public void SuppressGameplay()
    {
        waitForRelease = true;
        blockedThroughFrame = Time.frameCount + 1;
    }

    private bool AnyButtonHeld()
    {
        return AttackHeld ||
               WheelHeld ||
               (jump != null && jump.IsPressed()) ||
               (dash != null && dash.IsPressed()) ||
               (interact != null && interact.IsPressed());
    }

    public bool TryGetAim(
        Transform actor,
        out Vector3 direction)
    {
        if (UsingGamepad)
        {
            direction = hasStickAim
                ? lastStickAim
                : actor.forward;

            direction.y = 0f;

            if (direction.sqrMagnitude < 0.01f)
                direction = Vector3.forward;

            direction.Normalize();
            return true;
        }

        direction = Vector3.zero;

        if (aimCamera == null)
            aimCamera = Camera.main;

        if (aimCamera == null ||
            point == null ||
            point.activeControl == null)
        {
            return false;
        }

        Ray ray = aimCamera.ScreenPointToRay(
            point.ReadValue<Vector2>()
        );

        Plane plane = new Plane(Vector3.up, actor.position);

        if (!plane.Raycast(ray, out float distance))
            return false;

        direction = ray.GetPoint(distance) - actor.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f)
            return false;

        direction.Normalize();
        return true;
    }

    private void OnEnable()
    {
        InputSystem.onDeviceChange += DeviceChanged;
    }

    private void DeviceChanged(
        InputDevice device,
        InputDeviceChange change)
    {
        if (change == InputDeviceChange.Disconnected ||
            change == InputDeviceChange.Removed)
        {
            SuppressGameplay();
        }
    }

    private void OnApplicationFocus(bool focused)
    {
        if (!focused)
            SuppressGameplay();
    }

    private void OnDisable()
    {
        InputSystem.onDeviceChange -= DeviceChanged;
        SuppressGameplay();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}