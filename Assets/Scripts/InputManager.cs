using UnityEngine;

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
        float kbHorizontal = Input.GetAxis("Horizontal");
        bool kbJump = Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space);
        bool kbRestart = Input.GetKeyDown(KeyCode.R);

        // Touch fallback for simple control: first touch -> jump on tap, horizontal by touch position
        float touchHorizontal = 0f;
        bool touchJump = false;
        bool touchRestart = false;

        int touchCount = Input.touchCount;
        if (touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began) touchJump = true; // simple tap maps to jump

            // Horizontal by touch position relative to screen center
            float centerX = Screen.width * 0.5f;
            touchHorizontal = Mathf.Clamp((t.position.x - centerX) / centerX, -1f, 1f);

            // A two-finger tap could be used for restart (optional)
            if (touchCount >= 2)
            {
                for (int i = 0; i < touchCount; i++)
                {
                    if (Input.GetTouch(i).phase == TouchPhase.Began)
                    {
                        touchRestart = true;
                        break;
                    }
                }
            }
        }

        // Preference order: UI buttons (explicit), keyboard, touch
        Horizontal = uiHorizontal != 0f ? uiHorizontal : (Mathf.Abs(kbHorizontal) > 0.001f ? kbHorizontal : touchHorizontal);
        JumpDown = uiJumpPressed || kbJump || touchJump;
        RestartDown = uiRestartPressed || kbRestart || touchRestart;

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
