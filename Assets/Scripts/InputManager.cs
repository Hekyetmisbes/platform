using UnityEngine;
using UnityEngine.InputSystem;

// Simple input abstraction that supports keyboard/mouse and basic touch fallback.
// You can extend this to accept UI button calls from on-screen controls.
public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    // Public read-only properties other scripts will query each frame
    public float Horizontal { get; private set; }
    public bool JumpDown { get; private set; }
    public bool RestartDown { get; private set; }

    // These flags can be set by on-screen UI buttons (mobile)
    bool uiJumpPressed = false;
    bool uiRestartPressed = false;
    float uiHorizontal = 0f;

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

    void Update()
    {
        // Reset per-frame values
        JumpDown = false;
        RestartDown = false;

        // Desktop / Editor inputs (keyboard + axes)
        float kbHorizontal = 0f;
        bool kbJump = false;
        bool kbRestart = false;

        var keyboard = Keyboard.current;
        if (keyboard != null)
        {
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) kbHorizontal -= 1f;
            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) kbHorizontal += 1f;
            kbJump = keyboard.wKey.wasPressedThisFrame || keyboard.upArrowKey.wasPressedThisFrame || keyboard.spaceKey.wasPressedThisFrame;
            kbRestart = keyboard.rKey.wasPressedThisFrame;
        }

        float padHorizontal = 0f;
        bool padJump = false;
        bool padRestart = false;
        var gamepad = Gamepad.current;
        if (gamepad != null)
        {
            padHorizontal = gamepad.leftStick.x.ReadValue();
            padJump = gamepad.buttonSouth.wasPressedThisFrame;
            padRestart = gamepad.startButton.wasPressedThisFrame;
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

        // Preference order: UI buttons (explicit), keyboard, touch
        if (uiHorizontal != 0f)
        {
            Horizontal = uiHorizontal;
        }
        else if (Mathf.Abs(kbHorizontal) > 0.001f)
        {
            Horizontal = kbHorizontal;
        }
        else if (Mathf.Abs(padHorizontal) > 0.001f)
        {
            Horizontal = padHorizontal;
        }
        else
        {
            Horizontal = touchHorizontal;
        }

        JumpDown = uiJumpPressed || kbJump || padJump || touchJump;
        RestartDown = uiRestartPressed || kbRestart || padRestart || touchRestart;

        // Clear one-shot UI flags after consumed this frame only if they were used
        if (uiJumpPressed) uiJumpPressed = false;
        if (uiRestartPressed) uiRestartPressed = false;
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
}
