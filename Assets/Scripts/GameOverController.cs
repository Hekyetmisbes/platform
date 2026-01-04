using TMPro;
using UnityEngine;

public class GameOverController : MonoBehaviour
{
    [SerializeField] private GameObject gameoverUI;

    [SerializeField] private TextMeshProUGUI timeText;

    [SerializeField] private GameObject finishUI;

    [SerializeField] private Timer timer;

    [SerializeField] private PlayerController playerController;

    // Update is called once per frame
    void Update()
    {
        if (playerController != null)
        {
            if (playerController.IsDead)
            {
                GameOverObjects(gameoverUI);

                TimerControle();
            }
            if (playerController.IsFinish)
            {
                GameOverObjects(finishUI);

                TimerControle();
            }
        }
    }

    private void TimerControle()
    {
        if (timer != null)
        {
            timer.StopTimer();
        }
    }

    private void GameOverObjects(GameObject gameObject)
    {
        if (!gameObject.activeSelf)
        {
            ShowTime();
            gameObject.SetActive(true);
            timer.StopTimer();
        }
    }

    private void ShowTime()
    {
        timeText.text = "SCORE: " + timer.GetTime();
    }
}
