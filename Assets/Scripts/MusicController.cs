using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class MusicController : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] musicClips;
    [SerializeField] private TextMeshProUGUI musicNameText;

    [SerializeField] private GameObject musicUI;

    private int currentTrackIndex = 0;

    private static bool isPreloaded = false;

    void Start()
    {
        if (!isPreloaded)
        {
            StartCoroutine(PreloadMusicClips());
            isPreloaded = true;
        }
        PlayRandomTrack();
    }

    void Update()
    {
        bool nextTrackInput = false;
        var keyboard = Keyboard.current;
        if (keyboard != null)
        {
            nextTrackInput = keyboard.zKey.wasPressedThisFrame || keyboard.cKey.wasPressedThisFrame;
        }

        if ((!audioSource.isPlaying && musicClips.Length > 0) || nextTrackInput)
        {
            PlayRandomTrack();
        }
    }

    System.Collections.IEnumerator PreloadMusicClips()
    {
        if (musicClips == null || musicClips.Length == 0) yield break;

        foreach (var clip in musicClips)
        {
            if (clip == null) continue;

            if (clip.loadState == AudioDataLoadState.Unloaded)
            {
                clip.LoadAudioData();
            }

            while (clip.loadState == AudioDataLoadState.Loading)
            {
                yield return null; // spread load over frames to avoid a scene hitch
            }
        }
    }

    void PlayMusic(int index)
    {
        if (index >= 0 && index < musicClips.Length)
        {
            audioSource.clip = musicClips[index];
            audioSource.Play();
            musicUI.SetActive(true);
            musicNameText.text = musicClips[index].name;
            StartCoroutine(DisableMusicUIAfterAnimation());
        }
    }

    void PreviousTrack()
    {
        currentTrackIndex--;
        if (currentTrackIndex < 0)
        {
            currentTrackIndex = musicClips.Length - 1;
        }
        PlayMusic(currentTrackIndex);
    }

    void NextTrack()
    {
        currentTrackIndex++;
        if (currentTrackIndex >= musicClips.Length)
        {
            currentTrackIndex = 0;
        }
        PlayMusic(currentTrackIndex);
    }

    System.Collections.IEnumerator DisableMusicUIAfterAnimation()
    {
        yield return new WaitForSeconds(2f);
        musicUI.SetActive(false);
    }

    private int GetRandomTrackIndex()
    {
        return Random.Range(0, musicClips.Length);
    }

    public void PlayRandomTrack()
    {
        PlayMusic(GetRandomTrackIndex());
    }
}
