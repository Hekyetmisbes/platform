using TMPro;
using UnityEngine;

public class VolumeControl : MonoBehaviour
{
    private float volumeStep = 0.1f;
    private float maxVolume = 1.0f;
    private float minVolume = 0.0f;
    [SerializeField] private TextMeshProUGUI volumeText;

    private AudioManager audioManager;
    private const string LegacyVolumeKey = "Volume";

    void Start()
    {
        audioManager = AudioManager.Instance;
        float initialVolume = audioManager != null
            ? audioManager.MasterVolume
            : PlayerPrefs.GetFloat(LegacyVolumeKey, 1f);

        ApplyVolume(initialVolume, false);
    }

    public void IncreaseVolume()
    {
        float current = audioManager != null ? audioManager.MasterVolume : AudioListener.volume;
        float next = Mathf.Clamp(current + volumeStep, minVolume, maxVolume);
        ApplyVolume(next);
    }

    public void DecreaseVolume()
    {
        float current = audioManager != null ? audioManager.MasterVolume : AudioListener.volume;
        float next = Mathf.Clamp(current - volumeStep, minVolume, maxVolume);
        ApplyVolume(next);
    }

    private void ApplyVolume(float volume, bool save = true)
    {
        if (audioManager != null)
        {
            audioManager.SetMasterVolume(volume);
        }
        else
        {
            AudioListener.volume = volume;
            if (save)
            {
                PlayerPrefs.SetFloat("MasterVolume", AudioListener.volume);
                PlayerPrefs.SetFloat(LegacyVolumeKey, AudioListener.volume);
                PlayerPrefs.Save();
            }
        }

        if (volumeText != null)
        {
            volumeText.text = (volume * 100).ToString("0") + "%";
        }
    }
}
