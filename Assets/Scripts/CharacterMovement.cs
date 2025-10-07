using System;
using UnityEngine;

// Shared movement component for player-like characters.
// Handles horizontal movement, facing flip and animator speed parameter.
[RequireComponent(typeof(Rigidbody2D))]
public class CharacterMovement : MonoBehaviour
{
    [SerializeField] public float moveSpeed = 5f;

    Rigidbody2D rb;
    Animator animator;

    // Animator parameter cache
    private static readonly int PlayerSpeedHash = Animator.StringToHash("playerSpeed");

    // Expose facing state so other objects can mirror it
    public bool FacingRight { get; private set; } = true;

    // Event invoked when facing changes (parameter: FacingRight)
    public event Action<bool> OnFacingChanged;

    // Allow other code to read/write move speed without making the field public
    public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Initialize FacingRight according to the current visual scale so flip decisions match what's visible
        FacingRight = transform.localScale.x >= 0f;
    }

    // Drive horizontal movement from outside
    public void Move(float horizontal)
    {
        if (rb == null) return;

        // Use linearVelocity to match project Unity version and avoid recreating Vector2 when not needed
        Vector2 v = rb.linearVelocity;
        v.x = horizontal * moveSpeed;
        rb.linearVelocity = v;

        if (animator != null)
        {
            animator.SetFloat(PlayerSpeedHash, Mathf.Abs(v.x));
        }

        // Flip based on input so facing updates immediately when player presses left/right.
        const float deadzone = 0.01f;
        if (horizontal > deadzone && !FacingRight)
        {
            Flip();
        }
        else if (horizontal < -deadzone && FacingRight)
        {
            Flip();
        }
    }

    void Flip()
    {
        FacingRight = !FacingRight;
        Vector3 playerScale = transform.localScale;
        playerScale.x = Mathf.Abs(playerScale.x) * (FacingRight ? 1f : -1f);
        transform.localScale = playerScale;
        OnFacingChanged?.Invoke(FacingRight);
    }
}
