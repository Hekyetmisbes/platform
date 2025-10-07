using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] 
    TextMeshProUGUI timerText;

    private float time = 0f;

    private bool isTimerRunning = true;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        if(isTimerRunning)
        {
            time += Time.deltaTime;
            UpdateTimer();
        }
    }

    public void StopTimer()
    {
        isTimerRunning = false;

        Time.timeScale = 0f;
    }

    public void ResetTimer()
    {
        time = 0f;
        if (timerText != null) timerText.text = time.ToString("F2");
    }

    public void ResumeTimer()
    {
        isTimerRunning = true;
    }

    public void UpdateTimer()
    {
        if (timerText != null) timerText.text = time.ToString("F2");
    }

    public string GetTime()
    {
        return time.ToString("F2");
    }

    public float GetTimeF()
    {
        return time;
    }
}
