using UnityEngine;
using UnityEngine.Video;

public class IntroManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public VideoClip[] videoClips;

    private int currentVideoIndex = 0;

    void Start()
    {
        if (videoClips.Length == 0 || videoPlayer == null) gameObject.GetComponent<SceneTransition>().LoadScene("Main Menu");

        // play event when the last clip played
        videoPlayer.loopPointReached += OnVideoFinished;

        PlayVideo(currentVideoIndex);
    }

    void PlayVideo(int index)
    {
        if (index < videoClips.Length)
        {
            videoPlayer.clip = videoClips[index];
            videoPlayer.Play();
        }
        else
        {
            gameObject.GetComponent<SceneTransition>().LoadScene("Main Menu");
        }
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        currentVideoIndex++;
        if (currentVideoIndex < videoClips.Length)
        {
            PlayVideo(currentVideoIndex);
        }
        else
        {
            gameObject.GetComponent<SceneTransition>().LoadScene("Main Menu");
        }
    }
}
