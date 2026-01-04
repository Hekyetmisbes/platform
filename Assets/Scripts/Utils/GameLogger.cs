using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Centralized logging system with log levels and categories.
/// Provides conditional logging based on build configuration and category filtering.
/// </summary>
public enum LogLevel
{
    Verbose = 0,
    Info = 1,
    Warning = 2,
    Error = 3,
    None = 4
}

public enum LogCategory
{
    General,
    Input,
    Audio,
    Database,
    UI,
    Gameplay,
    Mobile
}

public static class GameLogger
{
    private static LogLevel currentLogLevel = LogLevel.Info;
    private static Dictionary<LogCategory, bool> categoryEnabled = new Dictionary<LogCategory, bool>();

    static GameLogger()
    {
        // Enable all categories by default in editor, only warnings/errors in build
        #if UNITY_EDITOR
        currentLogLevel = LogLevel.Verbose;
        #else
        currentLogLevel = LogLevel.Warning;
        #endif

        foreach (LogCategory category in System.Enum.GetValues(typeof(LogCategory)))
        {
            categoryEnabled[category] = true;
        }
    }

    /// <summary>
    /// Logs a message with specified category and level.
    /// Messages are filtered based on current log level and category settings.
    /// </summary>
    public static void Log(string message, LogCategory category = LogCategory.General, LogLevel level = LogLevel.Info)
    {
        if (level < currentLogLevel) return;
        if (!categoryEnabled.GetValueOrDefault(category, true)) return;

        string prefix = $"[{category}]";

        switch (level)
        {
            case LogLevel.Verbose:
            case LogLevel.Info:
                Debug.Log($"{prefix} {message}");
                break;
            case LogLevel.Warning:
                Debug.LogWarning($"{prefix} {message}");
                break;
            case LogLevel.Error:
                Debug.LogError($"{prefix} {message}");
                break;
        }
    }

    /// <summary>
    /// Sets the minimum log level to display.
    /// Messages below this level will be filtered out.
    /// </summary>
    public static void SetLogLevel(LogLevel level)
    {
        currentLogLevel = level;
        Log($"Log level set to {level}", LogCategory.General, LogLevel.Info);
    }

    /// <summary>
    /// Enables or disables logging for a specific category.
    /// </summary>
    public static void SetCategoryEnabled(LogCategory category, bool enabled)
    {
        categoryEnabled[category] = enabled;
    }

    /// <summary>
    /// Gets the current log level.
    /// </summary>
    public static LogLevel GetLogLevel()
    {
        return currentLogLevel;
    }

    /// <summary>
    /// Checks if a category is currently enabled for logging.
    /// </summary>
    public static bool IsCategoryEnabled(LogCategory category)
    {
        return categoryEnabled.GetValueOrDefault(category, true);
    }
}
