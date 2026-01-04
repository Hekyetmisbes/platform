using System;
using UnityEngine;

public enum SfxType
{
    Jump,
    Land,
    Death,
    Finish,
    Star,
    Button
}

/// <summary>
/// Central audio coordinator for music and sound effects.
/// Handles volume persistence, playlist control, and helper SFX hooks.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Configuration")]
    [SerializeField] private GameConfig config;
    [SerializeField] private bool autoPlayOnStart = true;
    [SerializeField] private bool loopPlaylist = true;
    [SerializeField] private bool shufflePlaylist = true;

    [Header("Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Music")]
    [SerializeField] private AudioClip[] musicTracks;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip landClip;
    [SerializeField] private AudioClip deathClip;
    [SerializeField] private AudioClip finishClip;
    [SerializeField] private AudioClip starClip;
    [SerializeField] private AudioClip buttonClip;

    [Header("Volumes")]
    [Range(0f, 1f)]
    [SerializeField] private float masterVolume = 1f;
    [Range(0f, 1f)]
    [SerializeField] private float musicVolume = 0.7f;
    [Range(0f, 1f)]
    [SerializeField] private float sfxVolume = 1f;

    private const string MasterPrefKey = "MasterVolume";
    private const string MusicPrefKey = "MusicVolume";
    private const string SfxPrefKey = "SFXVolume";

    private int currentTrackIndex = -1;

    public event Action<AudioClip, int> MusicTrackChanged;

    public float MasterVolume => masterVolume;
    public float MusicVolume => musicVolume;
    public float SfxVolume => sfxVolume;
    public string CurrentTrackName => currentTrackIndex >= 0 && currentTrackIndex < musicTracks.Length
        ? musicTracks[currentTrackIndex].name
        : string.Empty;
    public bool IsMusicPlaying => musicSource != null && musicSource.isPlaying;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        EnsureSources();
        LoadVolumeSettings();
        ApplyVolumeSettings();

        if (autoPlayOnStart)
        {
            PlayNextTrack(shufflePlaylist);
        }
    }

    private void Update()
    {
        if (loopPlaylist && musicTracks != null && musicTracks.Length > 0 && musicSource != null)
        {
            if (!musicSource.isPlaying && musicSource.clip != null)
            {
                PlayNextTrack(shufflePlaylist);
            }
        }
    }

    private void EnsureSources()
    {
        if (musicSource == null)
        {
            var musicObj = new GameObject("MusicSource");
            musicObj.transform.SetParent(transform);
            musicSource = musicObj.AddComponent<AudioSource>();
            musicSource.playOnAwake = false;
            musicSource.loop = false;
        }

        if (sfxSource == null)
        {
            var sfxObj = new GameObject("SFXSource");
            sfxObj.transform.SetParent(transform);
            sfxSource = sfxObj.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
            sfxSource.loop = false;
        }
    }

    private void LoadVolumeSettings()
    {
        // Use config defaults if preferences have not been set yet
        float defaultMusic = config != null ? config.musicVolume : musicVolume;
        float defaultSfx = config != null ? config.sfxVolume : sfxVolume;
        float defaultMaster = PlayerPrefs.HasKey(MasterPrefKey) ? masterVolume : 1f;

        masterVolume = PlayerPrefs.GetFloat(MasterPrefKey, defaultMaster);
        musicVolume = PlayerPrefs.GetFloat(MusicPrefKey, defaultMusic);
        sfxVolume = PlayerPrefs.GetFloat(SfxPrefKey, defaultSfx);
    }

    private void ApplyVolumeSettings()
    {
        AudioListener.volume = masterVolume;
        if (musicSource != null)
        {
            musicSource.volume = musicVolume * masterVolume;
        }
        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume * masterVolume;
        }
    }

    public void SetPlaylist(AudioClip[] tracks, bool autoPlay = false, bool shuffle = true)
    {
        musicTracks = tracks ?? Array.Empty<AudioClip>();
        if (autoPlay && musicTracks.Length > 0)
        {
            PlayNextTrack(shuffle);
        }
    }

    public void SetMusicSource(AudioSource source)
    {
        if (source == null) return;
        musicSource = source;
        musicSource.loop = false;
        ApplyVolumeSettings();
    }

    public void SetSfxSource(AudioSource source)
    {
        if (source == null) return;
        sfxSource = source;
        sfxSource.loop = false;
        ApplyVolumeSettings();
    }

    public void PlayNextTrack(bool shuffle = false)
    {
        if (musicTracks == null || musicTracks.Length == 0 || musicSource == null) return;

        if (shuffle)
        {
            int nextIndex = UnityEngine.Random.Range(0, musicTracks.Length);
            // Avoid repeating the same track if possible
            if (musicTracks.Length > 1)
            {
                int attempts = 0;
                while (nextIndex == currentTrackIndex && attempts < 3)
                {
                    nextIndex = UnityEngine.Random.Range(0, musicTracks.Length);
                    attempts++;
                }
            }
            currentTrackIndex = nextIndex;
        }
        else
        {
            currentTrackIndex = (currentTrackIndex + 1) % musicTracks.Length;
        }

        PlayMusic(currentTrackIndex);
    }

    public void PlayMusic(int index)
    {
        if (musicTracks == null || musicTracks.Length == 0 || musicSource == null) return;
        if (index < 0 || index >= musicTracks.Length) return;

        currentTrackIndex = index;
        musicSource.clip = musicTracks[index];
        musicSource.Play();
        MusicTrackChanged?.Invoke(musicSource.clip, currentTrackIndex);
    }

    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    public void PlaySfx(SfxType type, float volumeScale = 1f)
    {
        AudioClip clip = type switch
        {
            SfxType.Jump => jumpClip,
            SfxType.Land => landClip,
            SfxType.Death => deathClip,
            SfxType.Finish => finishClip,
            SfxType.Star => starClip,
            SfxType.Button => buttonClip,
            _ => null
        };

        PlaySfx(clip, volumeScale);
    }

    public void PlaySfx(AudioClip clip, float volumeScale = 1f)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume * volumeScale * masterVolume);
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat(MasterPrefKey, masterVolume);
        PlayerPrefs.Save();
        ApplyVolumeSettings();
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat(MusicPrefKey, musicVolume);
        PlayerPrefs.Save();
        ApplyVolumeSettings();
    }

    public void SetSfxVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat(SfxPrefKey, sfxVolume);
        PlayerPrefs.Save();
        ApplyVolumeSettings();
    }
}
