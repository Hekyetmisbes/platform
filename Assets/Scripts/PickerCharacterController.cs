using UnityEngine;

public class PickerCharacterController : MonoBehaviour
{
    public float moveSpeed = 5f;  // Hareket hýzý
    private Rigidbody2D rb;       // Rigidbody bileþeni
    private SpriteRenderer spriteRenderer; // SpriteRenderer bileþeni
    private bool canMoveRight = true;  // Saða hareket edebilir mi?
    private bool canMoveLeft = true;   // Sola hareket edebilir mi?

    void Start()
    {
        // Rigidbody2D ve SpriteRenderer bileþenlerini al
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Sað-sol hareketini almak için yatay input
        float moveInput = Input.GetAxis("Horizontal");

        // Hareket yönünü kontrol et
        if ((moveInput > 0 && canMoveRight) || (moveInput < 0 && canMoveLeft))
        {
            rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }

        // Yön deðiþiminde sprite'ý flip et
        if (moveInput < 0 && canMoveLeft)
        {
            spriteRenderer.flipX = true;
        }
        else if (moveInput > 0 && canMoveRight)
        {
            spriteRenderer.flipX = false;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Sað duvara çarptýysa saða hareketi durdur
        if (collision.gameObject.name == "BlindWallRight")
        {
            canMoveRight = false;
        }
        // Sol duvara çarptýysa sola hareketi durdur
        else if (collision.gameObject.name == "BlindWallLeft")
        {
            canMoveLeft = false;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // Sað duvardan ayrýldýðýnda saða hareket edebilir
        if (collision.gameObject.name == "BlindWallRight")
        {
            canMoveRight = true;
        }
        // Sol duvardan ayrýldýðýnda sola hareket edebilir
        else if (collision.gameObject.name == "BlindWallLeft")
        {
            canMoveLeft = true;
        }
    }
}
