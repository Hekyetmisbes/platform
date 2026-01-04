using UnityEngine;

/// <summary>
/// Singleton that caches references to commonly used scene objects.
/// Eliminates expensive FindObjectOfType calls throughout the codebase.
/// </summary>
public class SceneReferences : MonoBehaviour
{
    public static SceneReferences Instance { get; private set; }

    [Header("Managers")]
    [Tooltip("Reference to the InputManager in the scene")]
    public InputManager inputManager;

    [Tooltip("Reference to the Timer in the scene")]
    public Timer timer;

    [Tooltip("Reference to the CountdownManager in the scene")]
    public CountdownManager countdownManager;

    [Tooltip("Reference to the MobileControlManager in the scene")]
    public MobileControlManager mobileControlManager;

    [Header("UI")]
    [Tooltip("Reference to the pause menu GameObject")]
    public GameObject pauseMenu;

    [Tooltip("Reference to the HUD settings button")]
    public GameObject hudSettingsButton;

    [Tooltip("Reference to the mobile buttons root GameObject")]
    public GameObject buttonsRoot;

    [Tooltip("Reference to the mobile joystick root GameObject")]
    public GameObject joystickRoot;

    [Header("Gameplay")]
    [Tooltip("Reference to the PlayerController in the scene")]
    public PlayerController player;

    [Tooltip("Reference to the CameraController in the scene")]
    public CameraController cameraController;

    [Header("Auto-Find Settings")]
    [Tooltip("If enabled, will automatically find references that aren't assigned in the inspector")]
    [SerializeField] private bool autoFindMissingReferences = true;

    private void Awake()
    {
        // Singleton pattern - only one instance allowed
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple SceneReferences instances detected. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Auto-find missing references if enabled
        if (autoFindMissingReferences)
        {
            AutoFindReferences();
        }

        ValidateReferences();
    }

    /// <summary>
    /// Automatically finds references that aren't manually assigned in the inspector.
    /// Only finds references that are null to avoid overriding manual assignments.
    /// </summary>
    private void AutoFindReferences()
    {
        if (inputManager == null)
            inputManager = FindObjectOfType<InputManager>();

        if (timer == null)
            timer = FindObjectOfType<Timer>();

        if (countdownManager == null)
            countdownManager = FindObjectOfType<CountdownManager>();

        if (mobileControlManager == null)
            mobileControlManager = FindObjectOfType<MobileControlManager>();

        if (player == null)
            player = FindObjectOfType<PlayerController>();

        if (cameraController == null)
            cameraController = FindObjectOfType<CameraController>();

        // UI elements are typically found by name or tag if not assigned
        if (pauseMenu == null)
            pauseMenu = GameObject.Find("PauseMenu");

        if (hudSettingsButton == null)
            hudSettingsButton = GameObject.Find("HUDSettingsButton");

        if (buttonsRoot == null)
            buttonsRoot = GameObject.Find("MobileButtons");

        if (joystickRoot == null)
            joystickRoot = GameObject.Find("MobileJoystick");
    }

    /// <summary>
    /// Validates that critical references are assigned.
    /// Logs warnings for missing optional references.
    /// </summary>
    private void ValidateReferences()
    {
        // Warn about missing critical references
        if (inputManager == null)
            Debug.LogWarning("SceneReferences: InputManager reference is missing!");

        if (player == null)
            Debug.LogWarning("SceneReferences: PlayerController reference is missing!");

        // Optional references don't need warnings in all scenes
        // (some scenes may not have all components)
    }

    private void OnDestroy()
    {
        // Clear singleton instance when destroyed
        if (Instance == this)
        {
            Instance = null;
        }
    }

    /// <summary>
    /// Refreshes all auto-found references. Useful after scene changes or dynamic object creation.
    /// </summary>
    public void RefreshReferences()
    {
        if (autoFindMissingReferences)
        {
            AutoFindReferences();
            ValidateReferences();
        }
    }
}
