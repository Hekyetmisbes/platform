using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class LevelThemeEntry
{
    public int levelNumber = 1;
    public LevelTheme theme;
}

/// <summary>
/// Applies level-specific theming (colors, background sprite, optional music override).
/// Attach to a scene object and assign renderers you want themed.
/// </summary>
public class LevelThemeManager : MonoBehaviour
{
    [Header("Themes")]
    [SerializeField] private List<LevelThemeEntry> levelThemes = new List<LevelThemeEntry>();
    [SerializeField] private LevelTheme fallbackTheme;

    [Header("Targets")]
    [SerializeField] private Camera targetCamera;
    [SerializeField] private SpriteRenderer backgroundRenderer;
    [SerializeField] private List<SpriteRenderer> environmentRenderers = new List<SpriteRenderer>();

    [Header("Audio")]
    [SerializeField] private AudioManager audioManagerOverride;

    private void Start()
    {
        ApplyThemeForCurrentLevel();
    }

    public void ApplyThemeForCurrentLevel()
    {
        int levelNumber = Mathf.Max(SceneManager.GetActiveScene().buildIndex - 2, 1);
        LevelTheme theme = GetTheme(levelNumber) ?? fallbackTheme;
        ApplyTheme(theme);
    }

    public void ApplyTheme(LevelTheme theme)
    {
        if (theme == null) return;

        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        if (targetCamera != null)
        {
            targetCamera.backgroundColor = theme.backgroundColor;
        }

        if (backgroundRenderer != null)
        {
            backgroundRenderer.sprite = theme.backgroundSprite;
            backgroundRenderer.color = theme.accentColor;
        }

        if (environmentRenderers != null)
        {
            foreach (var renderer in environmentRenderers)
            {
                if (renderer == null) continue;

                renderer.color = theme.accentColor;
                if (theme.platformMaterial != null)
                {
                    renderer.sharedMaterial = theme.platformMaterial;
                }
            }
        }

        var audioManager = audioManagerOverride != null ? audioManagerOverride : AudioManager.Instance;
        if (audioManager != null && theme.musicOverride != null)
        {
            audioManager.SetPlaylist(new[] { theme.musicOverride }, true, false);
        }
    }

    private LevelTheme GetTheme(int levelNumber)
    {
        foreach (var entry in levelThemes)
        {
            if (entry != null && entry.theme != null && entry.levelNumber == levelNumber)
            {
                return entry.theme;
            }
        }

        return null;
    }
}
