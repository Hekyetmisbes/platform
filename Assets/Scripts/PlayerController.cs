using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D playerRB;

    Animator playerAnimator;

    CharacterMovement movement;
    SpriteRenderer spriteRenderer;

    private bool isGrounded = false;

    [SerializeField] private float moveSpeed = 5f;

    [SerializeField] private float jumpForce = 250f;

    private float jumpFrequency = 0.8f;
    private float nextJumpTime = 0f;

    [SerializeField] Transform groundCheckPosition;
    [SerializeField] float groundCheckRadius;
    [SerializeField] LayerMask groundLayer;

    private bool isFinish = false;
    private bool isDead = false;

    // Animator parameter hashes
    private static readonly int PlayerSpeedHash = Animator.StringToHash("playerSpeed");
    private static readonly int IsGroundedHash = Animator.StringToHash("isGrounded");

    void Start()
    {
        playerRB = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
        movement = GetComponent<CharacterMovement>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Use InputManager for platform-agnostic input
        var im = InputManager.Instance;

        HorizontalMove(im);

        GroundCheck();

        if (im != null)
        {
            if (im.JumpDown && isGrounded && (nextJumpTime < Time.timeSinceLevelLoad))
            {
                nextJumpTime = Time.timeSinceLevelLoad + jumpFrequency;
                Jump();
            }

            if (im.RestartDown)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
        else
        {
            // Fallback to Input System if no InputManager in scene
            bool jumpDown = false;
            bool restartDown = false;

            var keyboard = Keyboard.current;
            if (keyboard != null)
            {
                jumpDown = keyboard.wKey.wasPressedThisFrame || keyboard.upArrowKey.wasPressedThisFrame || keyboard.spaceKey.wasPressedThisFrame;
                restartDown = keyboard.rKey.wasPressedThisFrame;
            }

            var gamepad = Gamepad.current;
            if (gamepad != null)
            {
                jumpDown |= gamepad.buttonSouth.wasPressedThisFrame;
                restartDown |= gamepad.startButton.wasPressedThisFrame;
            }

            if (jumpDown && isGrounded && (nextJumpTime < Time.timeSinceLevelLoad))
            {
                nextJumpTime = Time.timeSinceLevelLoad + jumpFrequency;
                Jump();
            }

            if (restartDown)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }

    void HorizontalMove(InputManager im)
    {
        float horizontal = 0f;
        if (im != null)
        {
            horizontal = im.Horizontal;
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
            movement.Move(horizontal);
        }
        else
        {
            // fallback
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

    void Jump()
    {
        playerRB.AddForce(new Vector2(0f, jumpForce));
    }

    void GroundCheck()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheckPosition.position, groundCheckRadius, groundLayer);
        if (playerAnimator != null) playerAnimator.SetBool(IsGroundedHash, isGrounded);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Finish")) isFinish = true;
        if (collision.CompareTag("Dead")) isDead = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("ExtraJump"))
        {
            jumpForce *= 1.5f;
            jumpForce = Mathf.Min(jumpForce, 750f);
        }
        else if (collision.gameObject.CompareTag("LessJump"))
        {
            jumpForce *= 0.5f;
            jumpForce = Mathf.Max(jumpForce, 250f);
        }
    }

    public bool IsFinish => isFinish;
    public bool IsDead => isDead;
}
