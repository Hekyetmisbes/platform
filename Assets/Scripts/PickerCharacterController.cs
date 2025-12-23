using UnityEngine;
using UnityEngine.InputSystem;

public class PickerCharacterController : MonoBehaviour
{
    private float moveSpeed = 7.5f;

    private Rigidbody2D playerRB;

    private SpriteRenderer spriteRenderer;

    Animator playerAnimator;

    CharacterMovement movement;

    [SerializeField] private PauseMenuScript pauseMenuScript;
    
    void Start()
    {
        AudioListener.volume = 1f;
        Time.timeScale = 1;
        playerRB = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        movement = GetComponent<CharacterMovement>();
    }

    private static readonly int PlayerSpeedHash = Animator.StringToHash("playerSpeed");

    void Update()
    {
        HorizontalMove();
        var keyboard = Keyboard.current;
        if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame) pauseMenuScript?.EscapeControl();
    }

    void HorizontalMove()
    {
        float horizontal = 0f;
        if (InputManager.Instance != null)
        {
            horizontal = InputManager.Instance.Horizontal;
        }
        else
        {
            var keyboard = Keyboard.current;
            if (keyboard != null)
            {
                if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) horizontal -= 1f;
                if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) horizontal += 1f;
            }

            var gamepad = Gamepad.current;
            if (gamepad != null && Mathf.Abs(horizontal) < 0.001f)
            {
                horizontal = gamepad.leftStick.x.ReadValue();
            }
        }

        if (movement != null)
        {
            movement.moveSpeed = moveSpeed; // keep local speed
            movement.Move(horizontal);
        }
        else
        {
            Vector2 v = playerRB.linearVelocity;
            v.x = horizontal * moveSpeed;
            playerRB.linearVelocity = v;
            if (playerAnimator != null) playerAnimator.SetFloat(PlayerSpeedHash, Mathf.Abs(playerRB.linearVelocity.x));

            const float deadzone = 0.01f;
            if (spriteRenderer != null && Mathf.Abs(v.x) > deadzone)
            {
                spriteRenderer.flipX = v.x < 0f;
            }
            else if (Mathf.Abs(v.x) > deadzone)
            {
                Vector3 scale = transform.localScale;
                scale.x = Mathf.Abs(scale.x) * (v.x < 0f ? -1f : 1f);
                transform.localScale = scale;
            }
        }
    }

    // Flip handled by CharacterMovement component now
}
