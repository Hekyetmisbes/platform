using UnityEngine;

public class PickerCharacterController : MonoBehaviour
{
    public float moveSpeed = 5f;  // Hareket hýzý

    private Rigidbody2D playerRB;   // Rigidbody bileþeni

    private SpriteRenderer spriteRenderer;

    bool facingRight = true;

    Animator playerAnimator;

    void Start()
    {
        // Rigidbody2D ve SpriteRenderer bileþenlerini al
        playerRB = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        HorizontalMove();
    }

    void HorizontalMove()
    {
        playerRB.velocity = new Vector2(Input.GetAxis("Horizontal") * moveSpeed, playerRB.velocity.y);

        playerAnimator.SetFloat("playerSpeed", Mathf.Abs(playerRB.velocity.x));

        if (playerRB.velocity.x > 0 && !facingRight)
        {
            Flip();
        }
        else if (playerRB.velocity.x < 0 && facingRight)
        {
            Flip();
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 playerScale = transform.localScale;
        playerScale.x *= -1;
        transform.localScale = playerScale;
    }
}
