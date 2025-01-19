using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D playerRB;

    Animator playerAnimator;

    bool facingRight = true;

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

    // Start is called before the first frame update
    void Start()
    {
        playerRB = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        HorizontalMove();

        GroundCheck();

        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && isGrounded && (nextJumpTime < Time.timeSinceLevelLoad))
        {
            nextJumpTime = Time.timeSinceLevelLoad + jumpFrequency;
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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

    void Jump()
    {
        playerRB.AddForce(new Vector2(0f, jumpForce));
    }

    void GroundCheck()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheckPosition.position, groundCheckRadius, groundLayer);
        playerAnimator.SetBool("isGrounded", isGrounded);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Finish")
        {
            isFinish = true;
        }

        if (collision.gameObject.tag == "Dead")
        {
            isDead = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "ExtraJump")
        {
            jumpForce *= 1.5f;
            if (jumpForce > 750f)
            {
                jumpForce = 750f;
            }
        }
        if (collision.gameObject.tag == "LessJump")
        {
            jumpForce *= 0.5f;
            if (jumpForce < 250f)
            {
                jumpForce = 250f;
            }
        }
    }

    public bool IsFinish
    {
        get
        {
            return isFinish;
        }
    }

    public bool IsDead
    {
        get
        {
            return isDead;
        }
    }
}
