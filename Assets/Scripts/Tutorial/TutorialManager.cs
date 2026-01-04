using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class TutorialStep
{
    public string id;
    [TextArea] public string message;
    public bool waitForMove;
    public bool waitForJump;
    public bool waitForUseAction;
    public bool waitForFinish;
    public GameObject hintObject;
}

/// <summary>
/// Lightweight in-game tutorial controller.
/// Shows contextual steps on early levels and hides itself after completion.
/// </summary>
public class TutorialManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private CanvasGroup tutorialPanel;
    [SerializeField] private TextMeshProUGUI tutorialText;

    [Header("Steps")]
    [SerializeField] private List<TutorialStep> steps = new List<TutorialStep>();

    [Header("Behavior")]
    [SerializeField] private bool autoStartOnFirstLevel = true;
    [SerializeField] private int maxLevelNumberForAutoStart = 1;
    [SerializeField] private KeyCode skipKey = KeyCode.Tab;

    private int currentStepIndex = -1;
    private bool tutorialRunning;
    private InputManager inputManager;
    private PlayerController playerController;

    private const string TutorialCompletedKey = "TutorialCompleted";

    private void Start()
    {
        inputManager = InputManager.Instance != null ? InputManager.Instance : FindObjectOfType<InputManager>();
        playerController = SceneReferences.Instance != null ? SceneReferences.Instance.player : FindObjectOfType<PlayerController>();

        if (autoStartOnFirstLevel && ShouldRunTutorial())
        {
            StartTutorial();
        }
        else
        {
            HidePanel();
        }
    }

    private void Update()
    {
        if (!tutorialRunning) return;

        if (Input.GetKeyDown(skipKey))
        {
            SkipTutorial();
            return;
        }

        if (currentStepIndex < 0 || currentStepIndex >= steps.Count) return;

        if (IsStepCompleted(steps[currentStepIndex]))
        {
            Advance();
        }
    }

    private bool ShouldRunTutorial()
    {
        if (PlayerPrefs.GetInt(TutorialCompletedKey, 0) == 1) return false;
        int levelNumber = Mathf.Max(SceneManager.GetActiveScene().buildIndex - 2, 1);
        return levelNumber <= maxLevelNumberForAutoStart;
    }

    public void StartTutorial()
    {
        if (steps.Count == 0)
        {
            HidePanel();
            return;
        }

        tutorialRunning = true;
        currentStepIndex = 0;
        ShowStep(steps[currentStepIndex]);
    }

    public void SkipTutorial()
    {
        CompleteTutorial();
    }

    private void Advance()
    {
        currentStepIndex++;
        if (currentStepIndex >= steps.Count)
        {
            CompleteTutorial();
        }
        else
        {
            ShowStep(steps[currentStepIndex]);
        }
    }

    private void CompleteTutorial()
    {
        tutorialRunning = false;
        PlayerPrefs.SetInt(TutorialCompletedKey, 1);
        PlayerPrefs.Save();
        HidePanel();
    }

    private void ShowStep(TutorialStep step)
    {
        if (tutorialPanel != null)
        {
            tutorialPanel.alpha = 1f;
            tutorialPanel.blocksRaycasts = true;
            tutorialPanel.interactable = true;
        }

        if (tutorialText != null)
        {
            tutorialText.text = step.message;
        }

        ToggleHints(step);
        AudioManager.Instance?.PlaySfx(SfxType.Button, 0.6f);
    }

    private void HidePanel()
    {
        if (tutorialPanel != null)
        {
            tutorialPanel.alpha = 0f;
            tutorialPanel.blocksRaycasts = false;
            tutorialPanel.interactable = false;
        }

        ToggleHints(null);
    }

    private void ToggleHints(TutorialStep activeStep)
    {
        foreach (var step in steps)
        {
            if (step.hintObject != null)
            {
                step.hintObject.SetActive(activeStep != null && step == activeStep);
            }
        }
    }

    private bool IsStepCompleted(TutorialStep step)
    {
        if (step.waitForFinish && playerController != null && playerController.IsFinish) return true;

        if (inputManager != null)
        {
            if (step.waitForJump && inputManager.JumpDown) return true;
            if (step.waitForUseAction && inputManager.UseDown) return true;
            if (step.waitForMove && Mathf.Abs(inputManager.Horizontal) > 0.2f) return true;
        }
        else
        {
            // Fallback to direct input polling
            if (step.waitForJump && Input.GetButtonDown("Jump")) return true;
            if (step.waitForMove && Mathf.Abs(Input.GetAxis("Horizontal")) > 0.2f) return true;
        }

        return false;
    }
}
