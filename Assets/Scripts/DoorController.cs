using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

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
        var keyboard = Keyboard.current;
        if (isPlayerNear && keyboard != null && keyboard.eKey.wasPressedThisFrame) levelLoader?.LoadLevel(levelName);
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
