using UnityEngine;

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
        if (Input.GetKeyDown(KeyCode.Escape)) pauseMenuScript?.EscapeControl();
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
            horizontal = Input.GetAxis("Horizontal");
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
        }
    }

    // Flip handled by CharacterMovement component now
}
