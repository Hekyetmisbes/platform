using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class DoorController : MonoBehaviour
{
    public string levelName; // Kapýnýn temsil ettiði bölümün ismi
    private bool isPlayerNear = false; // Oyuncu kapýya yakýn mý?
    public TextMeshProUGUI uiText; // Ekranda gösterilecek uyarý (TextMeshPro)

    void Start()
    {
        // "EnterText" adlý TextMeshPro bileþenini bul ve baþlangýçta gizle
        // uiText = GameObject.Find("EnterText").GetComponent<TextMeshProUGUI>();
        /*if (uiText != null)
        {
            uiText.gameObject.SetActive(false);
        }*/
    }

    void Update()
    {
        // E tuþuna basýldýðýnda kapýya yakýnsa sahne geçiþini yap
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            SceneManager.LoadScene(levelName);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Karakter kapýya yaklaþtýðýnda uyarýyý göster
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
        // Karakter kapýdan uzaklaþtýðýnda uyarýyý gizle
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
