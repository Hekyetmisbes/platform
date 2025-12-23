using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class DoorController : MonoBehaviour
{
    [SerializeField] private string levelName;
    private bool isPlayerNear = false;
    [SerializeField] private TextMeshProUGUI uiText;

    [SerializeField] private LevelLoader levelLoader;
    [SerializeField] private string lockedMessage = "Locked: Earn 1 star in previous levels";
    [SerializeField] private GameObject lockIcon;
    private bool isLocked = false;

    void Update()
    {
        PlayScene();
    }

    void PlayScene()
    {
        var keyboard = Keyboard.current;
        if (isLocked) return;
        if (isPlayerNear && keyboard != null && keyboard.eKey.wasPressedThisFrame) levelLoader?.LoadLevel(levelName);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        isPlayerNear = true;
        if (uiText != null)
        {
            uiText.gameObject.SetActive(true);
            uiText.text = isLocked ? lockedMessage : "Press 'E' to Enter";
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        isPlayerNear = false;
        if (uiText != null) uiText.gameObject.SetActive(false);
    }

    public void SetLocked(bool locked, string message = null)
    {
        isLocked = locked;
        if (!string.IsNullOrWhiteSpace(message))
        {
            lockedMessage = message;
        }
        if (lockIcon != null) lockIcon.SetActive(isLocked);
    }
}
