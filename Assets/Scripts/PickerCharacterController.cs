using UnityEngine;

public class PickerCharacterController : MonoBehaviour
{
    private float moveSpeed = 7.5f;

    private Rigidbody2D playerRB;

    private SpriteRenderer spriteRenderer;

    bool facingRight = true;

    Animator playerAnimator;

    [SerializeField] private PauseMenuScript pauseMenuScript;
    
    void Start()
    {
        AudioListener.volume = 1f;
        Time.timeScale = 1;
        playerRB = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        HorizontalMove();
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            pauseMenuScript.EscapeControl();
        }
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
