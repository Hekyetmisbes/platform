using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// Simple input abstraction that supports keyboard/mouse and basic touch fallback.
// You can extend this to accept UI button calls from on-screen controls.
public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    [SerializeField] VirtualJoystick joystick;

    // Public read-only properties other scripts will query each frame
    public float Horizontal { get; private set; }
    public bool JumpDown { get; private set; }
    public bool RestartDown { get; private set; }
    public bool UseDown { get; private set; }

    // These flags can be set by on-screen UI buttons (mobile)
    bool uiJumpPressed = false;
    bool uiRestartPressed = false;
    bool uiUsePressed = false;
    float uiHorizontal = 0f;
    GameObject levelsButtonsRoot;
    GameObject levelsJoystickRoot;
    VirtualJoystick levelsJoystick;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
        MobileControlSettings.ModeChanged += HandleModeChanged;
        CacheJoystick();
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
        MobileControlSettings.ModeChanged -= HandleModeChanged;
    }

    void Update()
    {
        // Reset per-frame values
        JumpDown = false;
        RestartDown = false;
        UseDown = false;

        var mode = MobileControlSettings.CurrentMode;
        bool allowButtons = mode == MobileControlMode.Buttons;
        bool allowJoystick = mode == MobileControlMode.Joystick;
        bool allowJumpButton = allowButtons || allowJoystick;

        // Desktop / Editor inputs (keyboard + axes)
        float kbHorizontal = 0f;
        bool kbJump = false;
        bool kbRestart = false;
        bool kbUse = false;

        var keyboard = Keyboard.current;
        if (keyboard != null)
        {
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) kbHorizontal -= 1f;
            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) kbHorizontal += 1f;
            kbJump = keyboard.wKey.wasPressedThisFrame || keyboard.upArrowKey.wasPressedThisFrame || keyboard.spaceKey.wasPressedThisFrame;
            kbRestart = keyboard.rKey.wasPressedThisFrame;
            kbUse = keyboard.eKey.wasPressedThisFrame;
        }

        float padHorizontal = 0f;
        bool padJump = false;
        bool padRestart = false;
        bool padUse = false;
        var gamepad = Gamepad.current;
        if (gamepad != null)
        {
            padHorizontal = gamepad.leftStick.x.ReadValue();
            padJump = gamepad.buttonSouth.wasPressedThisFrame;
            padRestart = gamepad.startButton.wasPressedThisFrame;
            padUse = gamepad.buttonWest.wasPressedThisFrame;
        }

        // Joystick (UI)
        float joyHorizontal = 0f;
        if (allowJoystick && joystick != null)
        {
            joyHorizontal = joystick.Horizontal;
        }

        // Touch fallback for simple control: first touch -> jump on tap, horizontal by touch position
        float touchHorizontal = 0f;
        bool touchJump = false;
        bool touchRestart = false;

        var touch = Touchscreen.current;
        if (touch != null)
        {
            var primary = touch.primaryTouch;
            if (primary.press.wasPressedThisFrame) touchJump = true; // simple tap maps to jump

            // Horizontal by touch position relative to screen center
            if (primary.press.isPressed)
            {
                float centerX = Screen.width * 0.5f;
                float posX = primary.position.ReadValue().x;
                touchHorizontal = Mathf.Clamp((posX - centerX) / centerX, -1f, 1f);
            }

            // A two-finger tap could be used for restart (optional)
            int pressedTouches = 0;
            foreach (var touchControl in touch.touches)
            {
                if (touchControl.press.wasPressedThisFrame)
                {
                    pressedTouches++;
                }
                if (pressedTouches >= 2) break;
            }
            touchRestart = pressedTouches >= 2;
        }

        // Apply input priority: UI buttons > joystick > keyboard > gamepad > touch
        Horizontal = DetermineHorizontalInput(allowButtons, allowJoystick, uiHorizontal, joyHorizontal,
                                               kbHorizontal, padHorizontal, touchHorizontal);

        // Combine all jump inputs with mode filtering
        bool uiJump = allowJumpButton && uiJumpPressed;
        bool uiRestart = allowButtons && uiRestartPressed;
        bool uiUse = uiUsePressed;

        JumpDown = uiJump || kbJump || padJump || touchJump;
        RestartDown = uiRestart || kbRestart || padRestart || touchRestart;
        UseDown = uiUse || kbUse || padUse;

        // Clear one-shot UI flags after consumed this frame only if they were used
        if (uiJumpPressed) uiJumpPressed = false;
        if (uiRestartPressed) uiRestartPressed = false;
        if (uiUsePressed) uiUsePressed = false;
    }

    /// <summary>
    /// Determines horizontal input based on priority: UI > Joystick > Keyboard > Gamepad > Touch
    /// </summary>
    float DetermineHorizontalInput(bool allowButtons, bool allowJoystick, float ui, float joy,
                                    float kb, float pad, float touch)
    {
        const float deadzone = 0.001f;

        // Priority 1: UI Buttons (highest)
        float uiValue = allowButtons ? ui : 0f;
        if (Mathf.Abs(uiValue) > deadzone)
            return uiValue;

        // Priority 2: Virtual Joystick
        if (allowJoystick && Mathf.Abs(joy) > deadzone)
            return joy;

        // Priority 3: Keyboard
        if (Mathf.Abs(kb) > deadzone)
            return kb;

        // Priority 4: Gamepad
        if (Mathf.Abs(pad) > deadzone)
            return pad;

        // Priority 5: Touch (lowest)
        return touch;
    }

    // Methods for UI buttons to call (e.g. OnPointerDown/Up)
    public void SetUIHorizontal(float value)
    {
        uiHorizontal = Mathf.Clamp(value, -1f, 1f);
    }

    public void PressUIJump()
    {
        uiJumpPressed = true;
    }

    public void PressUIRestart()
    {
        uiRestartPressed = true;
    }

    public void PressUIUse()
    {
        uiUsePressed = true;
    }

    public void SetJoystick(VirtualJoystick value)
    {
        joystick = value;
    }

    void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CacheJoystick();
        if (scene.name == "Levels")
        {
            CacheLevelControls();
            ApplyLevelControls();
        }
    }

    void CacheJoystick()
    {
        if (joystick != null)
        {
            return;
        }
        joystick = FindInSceneVirtualJoystick();
    }

    void HandleModeChanged(MobileControlMode mode)
    {
        if (SceneManager.GetActiveScene().name == "Levels")
        {
            if (levelsButtonsRoot == null || levelsJoystickRoot == null)
            {
                CacheLevelControls();
            }
            ApplyLevelControls();
        }
    }

    void CacheLevelControls()
    {
        levelsButtonsRoot = FindInSceneByName("PlayButtons");
        levelsJoystickRoot = FindInSceneByName("VirtualJoystickUI");
        if (levelsJoystickRoot == null)
        {
            levelsJoystickRoot = FindInSceneByName("VirtualJoystick");
        }

        if (levelsJoystickRoot != null)
        {
            levelsJoystick = levelsJoystickRoot.GetComponentInChildren<VirtualJoystick>(true);
        }

        if (levelsJoystick == null)
        {
            levelsJoystick = FindInSceneVirtualJoystick();
        }
    }

    void ApplyLevelControls()
    {
        var mode = MobileControlSettings.CurrentMode;
        bool showButtons = mode == MobileControlMode.Buttons;
        bool showJoystick = mode == MobileControlMode.Joystick;

        if (levelsButtonsRoot != null) levelsButtonsRoot.SetActive(showButtons);
        if (levelsJoystickRoot != null) levelsJoystickRoot.SetActive(showJoystick);

        SetJoystick(showJoystick ? levelsJoystick : null);
    }

    static VirtualJoystick FindInSceneVirtualJoystick()
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

    static GameObject FindInSceneByName(string name)
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
}
