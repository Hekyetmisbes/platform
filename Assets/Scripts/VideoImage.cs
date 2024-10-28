using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoImage : MonoBehaviour
{
    public GameObject clickableObject; // Týklanabilir GameObject'i buraya sürükle
    public VideoPlayer videoPlayer; // Video Player'ý buraya sürükle
    public RawImage videoDisplay; // Videonun gösterileceði Raw Image'ý buraya sürükle

    void Start()
    {
        // EventTrigger bileþeni ekle
        EventTrigger trigger = clickableObject.AddComponent<EventTrigger>();

        // Týklama olayý için EventTrigger.Entry oluþtur
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((eventData) => { PlayVideo(); });

        // Entry'i EventTrigger'a ekle
        trigger.triggers.Add(entry);

        // Video Player'ýn loopPointReached olayýný dinle
        videoPlayer.loopPointReached += OnVideoFinished;

        // Baþlangýçta video görüntüsünü gizle
        videoDisplay.gameObject.SetActive(false);
    }

    void PlayVideo()
    {
        // Video Display'i görünür yap
        videoDisplay.gameObject.SetActive(true);

        // Video Player'ý baþlat
        videoPlayer.Play();
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        // Video Player durduðunda Raw Image'ý gizle
        videoDisplay.gameObject.SetActive(false);
    }
}
