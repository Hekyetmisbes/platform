#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class StarThresholdTuner : EditorWindow
{
    private class LevelThreshold
    {
        public int level;
        public float threeStar;
        public float twoStar;
        public float oneStar;
    }

    private readonly List<LevelThreshold> thresholds = new List<LevelThreshold>();
    private Vector2 scroll;
    private string databasePath = "StarsDatabase.db";
    private GameConfig config;
    private string statusMessage;

    [MenuItem("Tools/Star Threshold Tuner")]
    public static void ShowWindow()
    {
        GetWindow<StarThresholdTuner>("Star Threshold Tuner");
    }

    private void OnEnable()
    {
        LoadThresholds();
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Star Threshold Database", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        databasePath = EditorGUILayout.TextField("DB Path", databasePath);
        if (GUILayout.Button("Browse", GUILayout.Width(70)))
        {
            string picked = EditorUtility.OpenFilePanel("Select Database", Application.dataPath, "db");
            if (!string.IsNullOrEmpty(picked))
            {
                databasePath = picked;
            }
        }
        if (GUILayout.Button("Reload", GUILayout.Width(70)))
        {
            LoadThresholds();
        }
        EditorGUILayout.EndHorizontal();

        config = (GameConfig)EditorGUILayout.ObjectField("Game Config", config, typeof(GameConfig), false);

        EditorGUILayout.Space(6);
        scroll = EditorGUILayout.BeginScrollView(scroll);
        foreach (var row in thresholds)
        {
            DrawLevelRow(row);
            EditorGUILayout.Space(4);
        }
        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space(8);
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Save Thresholds", GUILayout.Width(160)))
        {
            SaveThresholds();
        }
        EditorGUILayout.EndHorizontal();

        if (!string.IsNullOrEmpty(statusMessage))
        {
            EditorGUILayout.HelpBox(statusMessage, MessageType.Info);
        }
    }

    private void DrawLevelRow(LevelThreshold row)
    {
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField($"Level {row.level}", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        row.threeStar = EditorGUILayout.FloatField("3 Star Time", row.threeStar);
        row.twoStar = EditorGUILayout.FloatField("2 Star Time", row.twoStar);
        row.oneStar = EditorGUILayout.FloatField("1 Star Time", row.oneStar);
        EditorGUILayout.EndHorizontal();

        if (Timer.TryGetLevelStats(row.level, out var stats))
        {
            EditorGUILayout.LabelField($"Recorded Runs: {stats.Runs} | Fastest: {stats.Fastest:F2}s | Avg: {stats.Average:F2}s | Slowest: {stats.Slowest:F2}s");
            var suggestion = Timer.GetSuggestedThresholds(stats, config);
            if (suggestion.ThreeStar > 0f)
            {
                EditorGUILayout.LabelField($"Suggested -> 3-star {suggestion.ThreeStar:F2}s | 2-star {suggestion.TwoStar:F2}s | 1-star {suggestion.OneStar:F2}s");
                if (GUILayout.Button("Apply Suggested", GUILayout.Width(130)))
                {
                    row.threeStar = suggestion.ThreeStar;
                    row.twoStar = suggestion.TwoStar;
                    row.oneStar = suggestion.OneStar;
                }
            }
            EditorGUILayout.Space(2);
        }
        else
        {
            EditorGUILayout.LabelField("Recorded Runs: None yet");
        }

        if (row.twoStar < row.threeStar)
        {
            EditorGUILayout.HelpBox("2 star threshold should be higher than 3 star threshold.", MessageType.Warning);
        }
        if (row.oneStar < row.twoStar)
        {
            EditorGUILayout.HelpBox("1 star threshold should be highest.", MessageType.Warning);
        }

        EditorGUILayout.EndVertical();
    }

    private string ResolveDatabasePath()
    {
        if (Path.IsPathRooted(databasePath))
        {
            return databasePath;
        }

        // Relative to project root
        string projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
        return Path.Combine(projectRoot, databasePath);
    }

    private void LoadThresholds()
    {
        thresholds.Clear();
        string dbPath = ResolveDatabasePath();

        if (!File.Exists(dbPath))
        {
            statusMessage = $"Database not found at {dbPath}";
            return;
        }

        try
        {
            var handler = new DatabaseHandler(dbPath);
            const string query = "SELECT BolumNumarasi, StarTime1, StarTime2, StarTime3 FROM BolumSureleri ORDER BY BolumNumarasi ASC;";
            using (var reader = handler.ExecuteReader(query))
            {
                while (reader.Read())
                {
                    thresholds.Add(new LevelThreshold
                    {
                        level = reader.GetInt32(0),
                        threeStar = reader.GetFloat(1),
                        twoStar = reader.GetFloat(2),
                        oneStar = reader.GetFloat(3)
                    });
                }
            }

            statusMessage = $"Loaded {thresholds.Count} level thresholds.";
        }
        catch (System.Exception ex)
        {
            statusMessage = $"Failed to load thresholds: {ex.Message}";
        }
    }

    private void SaveThresholds()
    {
        string dbPath = ResolveDatabasePath();
        if (!File.Exists(dbPath))
        {
            statusMessage = $"Database not found at {dbPath}";
            return;
        }

        try
        {
            var handler = new DatabaseHandler(dbPath);
            foreach (var row in thresholds)
            {
                const string updateQuery = "UPDATE BolumSureleri SET StarTime1 = @StarTime1, StarTime2 = @StarTime2, StarTime3 = @StarTime3 WHERE BolumNumarasi = @BolumNumarasi;";
                handler.ExecuteNonQuery(updateQuery,
                    ("@StarTime1", row.threeStar),
                    ("@StarTime2", row.twoStar),
                    ("@StarTime3", row.oneStar),
                    ("@BolumNumarasi", row.level));
            }

            statusMessage = "Thresholds saved to database.";
        }
        catch (System.Exception ex)
        {
            statusMessage = $"Failed to save thresholds: {ex.Message}";
        }
    }
}
#endif
