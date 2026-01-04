using UnityEngine;

/// <summary>
/// Centralized manager for mobile control UI and mode switching.
/// Consolidates control application logic from InputManager, CountdownManager, and MobileControlApplier.
/// </summary>
public class MobileControlManager : MonoBehaviour
{
    public static MobileControlManager Instance { get; private set; }

    [Header("Control Roots")]
    [SerializeField] private GameObject buttonsRoot;
    [SerializeField] private GameObject joystickRoot;
    [SerializeField] private VirtualJoystick virtualJoystick;

    [Header("Optional UI Elements")]
    [SerializeField] private GameObject hudSettingsButton;

    private bool controlsEnabled = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        CacheControlReferences();
    }

    private void OnEnable()
    {
        MobileControlSettings.ModeChanged += OnModeChanged;
    }

    private void OnDisable()
    {
        MobileControlSettings.ModeChanged -= OnModeChanged;
    }

    private void Start()
    {
        ApplyControlMode();
    }

    /// <summary>
    /// Enables or disables all mobile controls.
    /// </summary>
    public void EnableControls(bool enable)
    {
        controlsEnabled = enable;

        if (!enable)
        {
            // Hide all controls
            if (buttonsRoot != null) buttonsRoot.SetActive(false);
            if (joystickRoot != null) joystickRoot.SetActive(false);
            if (hudSettingsButton != null) hudSettingsButton.SetActive(false);

            // Clear joystick reference
            var inputManager = InputManager.Instance;
            if (inputManager != null)
            {
                inputManager.SetJoystick(null);
            }
        }
        else
        {
            // Apply current mode
            ApplyControlMode();
        }
    }

    /// <summary>
    /// Applies the current mobile control mode (Buttons or Joystick).
    /// </summary>
    public void ApplyControlMode()
    {
        if (!controlsEnabled)
        {
            return;
        }

        var mode = MobileControlSettings.CurrentMode;
        bool showButtons = mode == MobileControlMode.Buttons;
        bool showJoystick = mode == MobileControlMode.Joystick;

        // Toggle UI visibility
        if (buttonsRoot != null) buttonsRoot.SetActive(showButtons);
        if (joystickRoot != null) joystickRoot.SetActive(showJoystick);
        if (hudSettingsButton != null) hudSettingsButton.SetActive(true);

        // Update InputManager joystick reference
        var inputManager = InputManager.Instance;
        if (inputManager != null)
        {
            inputManager.SetJoystick(showJoystick ? virtualJoystick : null);
        }
    }

    /// <summary>
    /// Caches references to control UI elements if not assigned in inspector.
    /// </summary>
    private void CacheControlReferences()
    {
        if (buttonsRoot == null)
        {
            buttonsRoot = FindInSceneByName("PlayButtons");
        }

        if (joystickRoot == null)
        {
            joystickRoot = FindInSceneByName("VirtualJoystickUI");
            if (joystickRoot == null)
            {
                joystickRoot = FindInSceneByName("VirtualJoystick");
            }
        }

        if (hudSettingsButton == null)
        {
            hudSettingsButton = FindInSceneByName("HudSettingsButton");
        }

        if (virtualJoystick == null && joystickRoot != null)
        {
            virtualJoystick = joystickRoot.GetComponentInChildren<VirtualJoystick>(true);
        }

        if (virtualJoystick == null)
        {
            virtualJoystick = FindInSceneVirtualJoystick();
        }
    }

    private void OnModeChanged(MobileControlMode mode)
    {
        ApplyControlMode();
    }

    // Utility methods for finding objects in scene
    private static GameObject FindInSceneByName(string name)
    {
        var allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        for (int i = 0; i < allObjects.Length; i++)
        {
            var obj = allObjects[i];
            if (!obj.scene.IsValid())
            {
                continue;
            }
            if (obj.hideFlags != HideFlags.None)
            {
                continue;
            }
            if (obj.name == name)
            {
                return obj;
            }
        }
        return null;
    }

    private static VirtualJoystick FindInSceneVirtualJoystick()
    {
        var allJoysticks = Resources.FindObjectsOfTypeAll<VirtualJoystick>();
        for (int i = 0; i < allJoysticks.Length; i++)
        {
            var found = allJoysticks[i];
            if (found == null)
            {
                continue;
            }
            if (!found.gameObject.scene.IsValid())
            {
                continue;
            }
            if (found.hideFlags != HideFlags.None)
            {
                continue;
            }
            return found;
        }
        return null;
    }
}
