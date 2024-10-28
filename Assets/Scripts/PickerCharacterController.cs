using UnityEngine;

public class PickerCharacterController : MonoBehaviour
{
    public float moveSpeed = 5f;  // Hareket h�z�
    private Rigidbody2D rb;       // Rigidbody bile�eni
    private SpriteRenderer spriteRenderer; // SpriteRenderer bile�eni
    private bool canMoveRight = true;  // Sa�a hareket edebilir mi?
    private bool canMoveLeft = true;   // Sola hareket edebilir mi?

    void Start()
    {
        // Rigidbody2D ve SpriteRenderer bile�enlerini al
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Sa�-sol hareketini almak i�in yatay input
        float moveInput = Input.GetAxis("Horizontal");

        // Hareket y�n�n� kontrol et
        if ((moveInput > 0 && canMoveRight) || (moveInput < 0 && canMoveLeft))
        {
            rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }

        // Y�n de�i�iminde sprite'� flip et
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
        // Sa� duvara �arpt�ysa sa�a hareketi durdur
        if (collision.gameObject.name == "BlindWallRight")
        {
            canMoveRight = false;
        }
        // Sol duvara �arpt�ysa sola hareketi durdur
        else if (collision.gameObject.name == "BlindWallLeft")
        {
            canMoveLeft = false;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // Sa� duvardan ayr�ld���nda sa�a hareket edebilir
        if (collision.gameObject.name == "BlindWallRight")
        {
            canMoveRight = true;
        }
        // Sol duvardan ayr�ld���nda sola hareket edebilir
        else if (collision.gameObject.name == "BlindWallLeft")
        {
            canMoveLeft = true;
        }
    }
}
