using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoImage : MonoBehaviour
{
    public GameObject clickableObject; // T�klanabilir GameObject'i buraya s�r�kle
    public VideoPlayer videoPlayer; // Video Player'� buraya s�r�kle
    public RawImage videoDisplay; // Videonun g�sterilece�i Raw Image'� buraya s�r�kle

    void Start()
    {
        // EventTrigger bile�eni ekle
        EventTrigger trigger = clickableObject.AddComponent<EventTrigger>();

        // T�klama olay� i�in EventTrigger.Entry olu�tur
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((eventData) => { PlayVideo(); });

        // Entry'i EventTrigger'a ekle
        trigger.triggers.Add(entry);

        // Video Player'�n loopPointReached olay�n� dinle
        videoPlayer.loopPointReached += OnVideoFinished;

        // Ba�lang��ta video g�r�nt�s�n� gizle
        videoDisplay.gameObject.SetActive(false);
    }

    void PlayVideo()
    {
        // Video Display'i g�r�n�r yap
        videoDisplay.gameObject.SetActive(true);

        // Video Player'� ba�lat
        videoPlayer.Play();
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        // Video Player durdu�unda Raw Image'� gizle
        videoDisplay.gameObject.SetActive(false);
    }
}
