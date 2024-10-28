using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class DoorController : MonoBehaviour
{
    public string levelName; // Kap�n�n temsil etti�i b�l�m�n ismi
    private bool isPlayerNear = false; // Oyuncu kap�ya yak�n m�?
    public TextMeshProUGUI uiText; // Ekranda g�sterilecek uyar� (TextMeshPro)

    void Start()
    {
        // "EnterText" adl� TextMeshPro bile�enini bul ve ba�lang��ta gizle
        // uiText = GameObject.Find("EnterText").GetComponent<TextMeshProUGUI>();
        /*if (uiText != null)
        {
            uiText.gameObject.SetActive(false);
        }*/
    }

    void Update()
    {
        // E tu�una bas�ld���nda kap�ya yak�nsa sahne ge�i�ini yap
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            SceneManager.LoadScene(levelName);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Karakter kap�ya yakla�t���nda uyar�y� g�ster
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            if (uiText != null)
            {
                uiText.gameObject.SetActive(true);
                uiText.text = "Press 'E' for Enter";
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Karakter kap�dan uzakla�t���nda uyar�y� gizle
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            if (uiText != null)
            {
                uiText.gameObject.SetActive(false);
            }
        }
    }
}
