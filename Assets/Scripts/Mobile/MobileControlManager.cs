using UnityEngine;
using UnityEngine.SceneManagement;

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
            DontDestroyOnLoad(gameObject);
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
        SceneManager.sceneLoaded += OnSceneLoaded;
        MobileControlSettings.ModeChanged += OnModeChanged;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
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
    /// Forces a refresh of control references and re-applies the current mode.
    /// Useful when controls are initially inactive in the scene.
    /// </summary>
    public void RefreshAndApplyControls()
    {
        GameLogger.Log("Forcing control refresh and re-apply", LogCategory.Mobile, LogLevel.Info);
        CacheControlReferences(forceRefresh: true);
        ApplyControlMode();
    }

    /// <summary>
    /// Applies the current mobile control mode (Buttons or Joystick).
    /// Activates the appropriate controls based on user settings.
    /// </summary>
    public void ApplyControlMode()
    {
        if (!controlsEnabled)
        {
            GameLogger.Log("Controls are disabled, skipping ApplyControlMode", LogCategory.Mobile, LogLevel.Verbose);
            return;
        }

        var mode = MobileControlSettings.CurrentMode;
        bool showButtons = mode == MobileControlMode.Buttons;
        bool showJoystick = mode == MobileControlMode.Joystick;

        GameLogger.Log($"Applying control mode: {mode} (Buttons: {showButtons}, Joystick: {showJoystick})", LogCategory.Mobile, LogLevel.Info);

        // Always activate one control type and deactivate the other based on settings
        if (buttonsRoot != null)
        {
            bool wasActive = buttonsRoot.activeSelf;
            buttonsRoot.SetActive(showButtons);
            GameLogger.Log($"Buttons: {(wasActive ? "was active" : "was inactive")} -> now {(showButtons ? "active" : "inactive")}", LogCategory.Mobile, LogLevel.Info);
        }
        else if (showButtons)
        {
            GameLogger.Log("Button mode selected but buttonsRoot not found in scene", LogCategory.Mobile, LogLevel.Warning);
        }

        if (joystickRoot != null)
        {
            bool wasActive = joystickRoot.activeSelf;
            joystickRoot.SetActive(showJoystick);
            GameLogger.Log($"Joystick: {(wasActive ? "was active" : "was inactive")} -> now {(showJoystick ? "active" : "inactive")}", LogCategory.Mobile, LogLevel.Info);
        }
        else if (showJoystick)
        {
            GameLogger.Log("Joystick mode selected but joystickRoot not found in scene", LogCategory.Mobile, LogLevel.Warning);
        }

        if (hudSettingsButton != null)
        {
            hudSettingsButton.SetActive(true);
            GameLogger.Log("HUD settings button activated", LogCategory.Mobile, LogLevel.Verbose);
        }

        // Update InputManager joystick reference
        var inputManager = InputManager.Instance;
        if (inputManager != null)
        {
            inputManager.SetJoystick(showJoystick ? virtualJoystick : null);
            GameLogger.Log($"InputManager joystick reference {(showJoystick ? "set" : "cleared")}", LogCategory.Mobile, LogLevel.Verbose);
        }
        else
        {
            GameLogger.Log("InputManager instance not found", LogCategory.Mobile, LogLevel.Warning);
        }
    }

    /// <summary>
    /// Caches references to control UI elements if not assigned in inspector.
    /// </summary>
    /// <param name="forceRefresh">If true, re-finds all controls even if cached</param>
    private void CacheControlReferences(bool forceRefresh = false)
    {
        if (buttonsRoot == null || forceRefresh)
        {
            buttonsRoot = FindInSceneByName("PlayButtons");
        }

        if (joystickRoot == null || forceRefresh)
        {
            joystickRoot = FindInSceneByName("VirtualJoystickUI");
            if (joystickRoot == null)
            {
                joystickRoot = FindInSceneByName("VirtualJoystick");
            }
        }

        if (hudSettingsButton == null || forceRefresh)
        {
            hudSettingsButton = FindInSceneByName("HudSettingsButton");
        }

        if (virtualJoystick == null || forceRefresh)
        {
            if (joystickRoot != null)
            {
                virtualJoystick = joystickRoot.GetComponentInChildren<VirtualJoystick>(true);
            }

            if (virtualJoystick == null)
            {
                virtualJoystick = FindInSceneVirtualJoystick();
            }
        }

        GameLogger.Log($"Mobile controls cached - Buttons: {buttonsRoot != null}, Joystick: {joystickRoot != null}, VirtualJoystick: {virtualJoystick != null}", LogCategory.Mobile, LogLevel.Verbose);
    }

    /// <summary>
    /// Called when a new scene is loaded. Re-caches controls and applies mode.
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameLogger.Log($"Scene loaded: {scene.name}, re-caching mobile controls", LogCategory.Mobile, LogLevel.Info);

        // Use coroutine to ensure scene is fully loaded before finding controls
        StartCoroutine(ApplyControlsAfterSceneLoad());
    }

    /// <summary>
    /// Coroutine to apply controls after scene has fully loaded.
    /// Waits one frame to ensure all scene objects are initialized.
    /// </summary>
    private System.Collections.IEnumerator ApplyControlsAfterSceneLoad()
    {
        // Wait for end of frame to ensure scene is fully loaded
        yield return new UnityEngine.WaitForEndOfFrame();

        // Force refresh control references for the new scene
        CacheControlReferences(forceRefresh: true);

        // Apply control mode to new scene's controls (will activate them based on settings)
        ApplyControlMode();

        GameLogger.Log("Mobile controls applied after scene load", LogCategory.Mobile, LogLevel.Info);
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
