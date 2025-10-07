using UnityEngine;
using TMPro;

public class DoorController : MonoBehaviour
{
    [SerializeField] private string levelName;
    private bool isPlayerNear = false;
    [SerializeField] private TextMeshProUGUI uiText;

    [SerializeField] private LevelLoader levelLoader;

    void Update()
    {
        PlayScene();
    }

    void PlayScene()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E)) levelLoader?.LoadLevel(levelName);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        isPlayerNear = true;
        if (uiText != null)
        {
            uiText.gameObject.SetActive(true);
            uiText.text = "Press 'E' to Enter";
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        isPlayerNear = false;
        if (uiText != null) uiText.gameObject.SetActive(false);
    }
}
