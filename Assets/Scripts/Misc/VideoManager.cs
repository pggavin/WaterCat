using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine;

// This is used for the end cutscene
public class VideoManager : MonoBehaviour
{
    public string _sceneAfterPlaying;

    void Awake()
    {
        var player = GetComponent<VideoPlayer>();

        player.loopPointReached += done => {
            SceneManager.LoadScene(_sceneAfterPlaying);
        };
        // We change to the scene specified once the video is done playing
    }
}