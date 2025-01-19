using TMPro;
using UnityEngine;

public class VolumeControl : MonoBehaviour
{
    private float volumeStep = 0.1f;
    private float maxVolume = 1.0f;
    private float minVolume = 0.0f;
    [SerializeField] private TextMeshProUGUI volumeText;

    void Start()
    {
        AudioListener.volume = PlayerPrefs.GetFloat("Volume");
        volumeText.text = (AudioListener.volume * 100).ToString("0") + "%";
    }

    void Update()
    {
        AudioListener.volume = PlayerPrefs.GetFloat("Volume");
    }

    public void IncreaseVolume()
    {
        AudioListener.volume = Mathf.Clamp(AudioListener.volume + volumeStep, minVolume, maxVolume);
        SaveVolume();
        volumeText.text = (AudioListener.volume * 100).ToString("0") + "%";
    }

    public void DecreaseVolume()
    {
        AudioListener.volume = Mathf.Clamp(AudioListener.volume - volumeStep, minVolume, maxVolume);
        SaveVolume();
        volumeText.text = (AudioListener.volume * 100).ToString("0") + "%";
    }

    private void SaveVolume()
    {
        PlayerPrefs.SetFloat("Volume", AudioListener.volume);
        PlayerPrefs.Save();
    }
}
