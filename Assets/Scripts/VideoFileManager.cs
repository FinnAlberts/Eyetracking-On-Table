using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

public class VideoFileManager : MonoBehaviour
{
    /// <summary>
    /// The video player
    /// </summary>
    VideoPlayer videoPlayer;

    /// <summary>
    /// The current video frame
    /// </summary>
    Texture2D videoFrame;

    /// <summary>
    /// URL of the video to be played (can also be local file)
    /// </summary>
    public string videoURL;

    /// <summary>
    /// Speed at which the video should be played
    /// </summary>
    public float playSpeed;

    /// <summary>
    /// Event to be trigger every new frame with the new timestamp
    /// </summary>
    public UnityEvent<double> onNewTimestamp;

    // Called before the first frame
    void Start()
    {
        // Check if using video file or webcam
        if (!DetectorManager.Instance.useFileVideo)
        {
            return;
        }

        // Initialize videoFrame variable
        videoFrame = new Texture2D(2, 2);

        // Will attach a video player to the main camera.
        GameObject camera = Camera.main.gameObject;

        // Attach video player to camera
        videoPlayer = camera.AddComponent<UnityEngine.Video.VideoPlayer>();

        // Play on awake defaults to true. Set it to false to avoid the url set below to auto-start playback since we're in Start()
        videoPlayer.playOnAwake = false;

        // Set the video to play
        videoPlayer.url = videoURL;

        // Set the playspeed of the video
        videoPlayer.playbackSpeed = playSpeed;

        // Stop video on video end
        videoPlayer.loopPointReached += EndReached;

        // Trigger an event every new frame
        videoPlayer.sendFrameReadyEvents = true;

        videoPlayer.frameReady += OnNewFrame;

        // Disable skipping of frames
        videoPlayer.skipOnDrop = false;

        // Start video
        videoPlayer.Play();
    }

    void EndReached(UnityEngine.Video.VideoPlayer videoPlayer)
    {
        videoPlayer.Stop();
    }

    // Called every new video frame
    void OnNewFrame(VideoPlayer _source, long _frameIdx)
    {
        // Pause the video untill data is processed
        _source.Pause();
        
        // Trigger onNewTimestamp event with current time
        onNewTimestamp?.Invoke(_source.time);

        // Convert frame to Texture2D
        RenderTexture renderTexture = _source.texture as RenderTexture;

        if (videoFrame.width != renderTexture.width || videoFrame.height != renderTexture.height)
        {
            videoFrame.Resize(renderTexture.width, renderTexture.height);
        }

        RenderTexture.active = renderTexture;

        videoFrame.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        videoFrame.Apply();

        RenderTexture.active = null;

        // Update the AprilTags
        DetectorManager.Instance.UpdateApriltags(videoFrame.GetPixels32());
    }

    /// <summary>
    /// Resume playing the video
    /// </summary>
    public void ResumeVideo()
    {
        videoPlayer.Play();
    }
}
