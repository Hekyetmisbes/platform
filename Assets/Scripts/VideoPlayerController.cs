using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoPlayerController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public VideoClip video1;
    public VideoClip video2;
    public VideoClip video3;
    public VideoClip video4;
    public RawImage rawImage;

    // Hangi objeye týklanýldýðýný anlamak için deðiþken
    private int videoIndex;

    private void Start()
    {
        // Objeye göre videoIndex ayarlayýn
        switch (gameObject.name)
        {
            case "TutorialVideo":
                videoIndex = 1;
                break;
            case "BeliveVideo":
                videoIndex = 2;
                break;
            case "VSVideo":
                videoIndex = 3;
                break;
            case "GameOverVideo":
                videoIndex = 4;
                break;
        }
    }

    private void Update()
    {
        // Eðer E tuþuna basýlýrsa videoyu durdur
        if (Input.GetKeyDown(KeyCode.E) && videoPlayer.isPlaying)
        {
            StopVideo();
        }
    }

    private void OnMouseDown()
    {
        rawImage.gameObject.SetActive(true);
        PlayVideo(videoIndex);
    }

    private void PlayVideo(int index)
    {
        switch (index)
        {
            case 1:
                videoPlayer.clip = video1;
                break;
            case 2:
                videoPlayer.clip = video2;
                break;
            case 3:
                videoPlayer.clip = video3;
                break;
            case 4:
                videoPlayer.clip = video4;
                break;
        }

        videoPlayer.Play();
        videoPlayer.loopPointReached += EndReached;
    }

    private void EndReached(VideoPlayer vp)
    {
        videoPlayer.Stop();
        rawImage.gameObject.SetActive(false);
    }

    private void StopVideo()
    {
        videoPlayer.Stop();
        rawImage.gameObject.SetActive(false);
    }
}
