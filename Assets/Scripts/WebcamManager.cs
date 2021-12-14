using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UI = UnityEngine.UI;

/// <summary>
/// Manage the webcam and its preview
/// </summary>
public class WebcamManager : MonoBehaviour
{
    /// <summary>
    /// Event for when detected Apriltags have been updated
    /// </summary>
    public UnityEvent<List<AprilTag.TagPose>> onDetectedApriltagsUpdated;

    /// <summary>
    /// Preview of the webcam (raw input)
    /// </summary>
    [SerializeField] UI.RawImage webcamPreview = null;

    /// <summary>
    /// Raw video input of the webcam
    /// </summary>
    WebCamTexture webcamRaw;

    /// <summary>
    /// Buffer for storing current frame of webcam stream to use for webcam previewing
    /// </summary>
    RenderTexture webcamBuffer;

    /// <summary>
    /// 
    /// </summary>
    Color32[] readBuffer;

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        if (DetectorManager.Instance.useFileVideo)
        {
            return;
        }
        
        // Webcam initialization
        webcamRaw = new WebCamTexture(DetectorManager.Instance.resolution.x, DetectorManager.Instance.resolution.y, 60);
        webcamBuffer = new RenderTexture(DetectorManager.Instance.resolution.x, DetectorManager.Instance.resolution.y, 0);
        readBuffer = new Color32[DetectorManager.Instance.resolution.x * DetectorManager.Instance.resolution.y];

        // Start webcam and preview
        webcamRaw.Play();
        webcamPreview.texture = webcamBuffer;
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        // Webcam image buffering
        webcamRaw.GetPixels32(readBuffer);
        Graphics.Blit(webcamRaw, webcamBuffer);

        // Update the Apriltags
        DetectorManager.Instance.UpdateApriltags(readBuffer);
    }

    /// <summary>
    /// Called on destroy
    /// </summary>
    void OnDestroy()
    {
        Destroy(webcamRaw);
        Destroy(webcamBuffer);
    }
}
