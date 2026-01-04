using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class Timer : MonoBehaviour
{
    [SerializeField] 
    TextMeshProUGUI timerText;
    [SerializeField] private GameConfig config;

    private float time = 0f;

    private bool isTimerRunning = true;
    private static readonly Dictionary<int, LevelTimeStats> levelStats = new Dictionary<int, LevelTimeStats>();

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
        // Removed Time.timeScale = 0f to prevent freezing UI animations and other game systems
        // Only the timer itself stops now, not the entire game
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

    /// <summary>
    /// Records a level completion time for analytics and star threshold tuning.
    /// </summary>
    public void RecordLevelTime(int levelNumber, float finishTime)
    {
        if (finishTime < 0f || levelNumber <= 0) return;

        if (!levelStats.TryGetValue(levelNumber, out var stats))
        {
            stats = new LevelTimeStats
            {
                Fastest = finishTime,
                Slowest = finishTime,
                TotalTime = finishTime,
                Runs = 1,
                LastTime = finishTime
            };
        }
        else
        {
            stats.Fastest = Mathf.Min(stats.Fastest, finishTime);
            stats.Slowest = Mathf.Max(stats.Slowest, finishTime);
            stats.TotalTime += finishTime;
            stats.Runs += 1;
            stats.LastTime = finishTime;
        }

        levelStats[levelNumber] = stats;

        var suggestion = GetSuggestedThresholds(levelNumber, config);
        GameLogger.Log($"Level {levelNumber} times -> Min={stats.Fastest:F2}s Avg={stats.Average:F2}s Max={stats.Slowest:F2}s Runs={stats.Runs} " +
                       $"Suggested thresholds: 3-star={suggestion.ThreeStar:F2}s 2-star={suggestion.TwoStar:F2}s 1-star={suggestion.OneStar:F2}s",
                       LogCategory.Gameplay, LogLevel.Info);
    }

    public static bool TryGetLevelStats(int levelNumber, out LevelTimeStats stats)
    {
        return levelStats.TryGetValue(levelNumber, out stats);
    }

    public static StarThresholdSuggestion GetSuggestedThresholds(int levelNumber, GameConfig config = null)
    {
        if (!levelStats.TryGetValue(levelNumber, out var stats))
        {
            return default;
        }

        return GetSuggestedThresholds(stats, config);
    }

    public static StarThresholdSuggestion GetSuggestedThresholds(LevelTimeStats stats, GameConfig config = null)
    {
        if (stats.Fastest <= 0f)
        {
            return default;
        }

        float threeStar = stats.Fastest * (config != null ? config.threeStarMultiplier : 1.1f);
        float twoStar = stats.Fastest * (config != null ? config.twoStarMultiplier : 1.4f);
        float oneStar = stats.Fastest * (config != null ? config.oneStarMultiplier : 2.0f);

        return new StarThresholdSuggestion
        {
            ThreeStar = threeStar,
            TwoStar = twoStar,
            OneStar = oneStar
        };
    }
}

public struct LevelTimeStats
{
    public float Fastest;
    public float Slowest;
    public float TotalTime;
    public int Runs;
    public float LastTime;

    public float Average => Runs > 0 ? TotalTime / Runs : 0f;
}

public struct StarThresholdSuggestion
{
    public float ThreeStar;
    public float TwoStar;
    public float OneStar;
}
