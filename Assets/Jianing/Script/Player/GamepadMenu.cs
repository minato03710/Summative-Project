using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class GamepadMenu : MonoBehaviour
{
    public Selectable firstSelected;
    public UnityEvent onCancel = new UnityEvent();

    private static readonly List<GamepadMenu> openMenus =
        new List<GamepadMenu>();

    private int openedFrame;
    private static int canceledFrame = -1;

    public static bool AnyOpen
    {
        get
        {
            openMenus.RemoveAll(
                menu => menu == null || !menu.isActiveAndEnabled
            );

            return openMenus.Count > 0;
        }
    }

    public static EventSystem EnsureEventSystem()
    {
        EventSystem system = EventSystem.current;

        if (system == null)
            system = FindFirstObjectByType<EventSystem>();

        if (system == null)
        {
            system = new GameObject(
                "EventSystem",
                typeof(EventSystem)
            ).GetComponent<EventSystem>();
        }

        StandaloneInputModule oldModule =
            system.GetComponent<StandaloneInputModule>();

        if (oldModule != null)
            oldModule.enabled = false;

        InputSystemUIInputModule module =
            system.GetComponent<InputSystemUIInputModule>();

        if (module == null)
        {
            module = system.gameObject
                .AddComponent<InputSystemUIInputModule>();

            module.AssignDefaultActions();
        }

        module.enabled = true;
        module.deselectOnBackgroundClick = false;

        PlayerInputReader reader = PlayerInputReader.Instance;

        if (reader != null && reader.Input != null)
            reader.Input.uiInputModule = module;

        system.sendNavigationEvents = true;

        return system;
    }

    private void OnEnable()
    {
        openMenus.Remove(this);
        openMenus.Add(this);

        openedFrame = Time.frameCount;

        EnsureEventSystem();

        if (PlayerInputReader.Instance != null)
            PlayerInputReader.Instance.SuppressGameplay();

        foreach (Selectable item in
                 GetComponentsInChildren<Selectable>(true))
        {
            Navigation navigation = item.navigation;

            if (navigation.mode != Navigation.Mode.None)
                continue;

            navigation.mode = Navigation.Mode.Automatic;
            item.navigation = navigation;
        }

        SelectFirst();
    }

    private void Update()
    {
        if (!AnyOpen ||
            openMenus[openMenus.Count - 1] != this)
        {
            return;
        }

        EventSystem system = EventSystem.current;

        if (system == null)
            system = EnsureEventSystem();

        system.sendNavigationEvents = true;

        GameObject selected = system.currentSelectedGameObject;

        Selectable item = selected != null
            ? selected.GetComponent<Selectable>()
            : null;

        if (item == null ||
            !item.transform.IsChildOf(transform) ||
            !item.isActiveAndEnabled ||
            !item.IsInteractable())
        {
            SelectFirst();
        }

        InputSystemUIInputModule module =
            system.GetComponent<InputSystemUIInputModule>();

        if (Time.frameCount > openedFrame &&
            canceledFrame != Time.frameCount &&
            module != null &&
            module.cancel != null &&
            module.cancel.action != null &&
            module.cancel.action.WasPressedThisFrame())
        {
            canceledFrame = Time.frameCount;
            onCancel.Invoke();
        }
    }

    private void SelectFirst()
    {
        EventSystem system = EventSystem.current;

        if (system == null)
            return;

        if (firstSelected != null &&
            firstSelected.isActiveAndEnabled &&
            firstSelected.IsInteractable())
        {
            system.SetSelectedGameObject(
                firstSelected.gameObject
            );

            return;
        }

        foreach (Selectable item in
                 GetComponentsInChildren<Selectable>())
        {
            if (!item.isActiveAndEnabled ||
                !item.IsInteractable())
            {
                continue;
            }

            system.SetSelectedGameObject(item.gameObject);
            return;
        }

        system.SetSelectedGameObject(null);
    }

    private void OnDisable()
    {
        openMenus.Remove(this);

        EventSystem system = EventSystem.current;

        if (system != null &&
            system.currentSelectedGameObject != null &&
            system.currentSelectedGameObject.transform
                .IsChildOf(transform))
        {
            system.SetSelectedGameObject(null);
        }

        if (PlayerInputReader.Instance != null)
            PlayerInputReader.Instance.SuppressGameplay();

        if (system != null)
        {
            system.sendNavigationEvents =
                AnyOpen || Time.timeScale <= 0f;
        }
    }
}