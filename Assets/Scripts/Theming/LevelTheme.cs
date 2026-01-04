using UnityEngine;

[CreateAssetMenu(fileName = "LevelTheme", menuName = "Game/Level Theme")]
public class LevelTheme : ScriptableObject
{
    [Header("Colors")]
    public Color backgroundColor = Color.black;
    public Color accentColor = Color.cyan;

    [Header("Visuals")]
    public Sprite backgroundSprite;
    public Material platformMaterial;

    [Header("Audio")]
    public AudioClip musicOverride;
}
