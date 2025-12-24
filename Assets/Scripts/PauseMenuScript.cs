using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenuScript : MonoBehaviour
{
    [SerializeField] private GameObject player;
    public GameObject pauseMenuUI;
    public GameObject settingsMenuUI;
    [SerializeField] GameObject gameoverUI;
    [SerializeField] GameObject finishUI;
    [SerializeField] GameObject hudSettingsButton;
    private bool isPaused = false;

    void Awake()
    {
        if (hudSettingsButton == null)
        {
            var found = GameObject.Find("HudSettingsButton");
            if (found != null)
            {
                hudSettingsButton = found;
            }
        }
    }

    void Update()
    {
        // Only check for escape if player object is assigned
        if (player != null) EscapeControl();
    }

    public void EscapeControl()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null || !keyboard.escapeKey.wasPressedThisFrame || !player.activeSelf) return;
        if (isPaused) ResumeGame(); else PauseGame();
    }

    private string GetActiveSceneName()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        return sceneName;
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        settingsMenuUI.SetActive(false);
        if (hudSettingsButton != null) hudSettingsButton.SetActive(true);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void PauseGame()
    {
        if (gameoverUI.activeSelf || finishUI.activeSelf)
        {
            return;
        }
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void OpenSettings()
    {
        pauseMenuUI.SetActive(false);
        settingsMenuUI.SetActive(true);
    }

    public void OpenSettingsFromHUD()
    {
        if (gameoverUI != null && gameoverUI.activeSelf)
        {
            return;
        }
        if (finishUI != null && finishUI.activeSelf)
        {
            return;
        }

        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        if (settingsMenuUI != null) settingsMenuUI.SetActive(true);
        if (hudSettingsButton != null) hudSettingsButton.SetActive(false);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void CloseSettings()
    {
        settingsMenuUI.SetActive(false);
        pauseMenuUI.SetActive(true);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void NextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(GetActiveSceneName());
    }
}
