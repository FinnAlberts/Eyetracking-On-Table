using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UI = UnityEngine.UI;

public class WebcamManager : MonoBehaviour
{
    /// <summary>
    /// Event for when detected Apriltags have been updated
    /// </summary>
    public UnityEvent<List<AprilTag.TagPose>> onDetectedApriltagsUpdated;

    [SerializeField] UI.RawImage webcamPreview = null;

    WebCamTexture webcamRaw;
    RenderTexture webcamBuffer;
    Color32[] readBuffer;

    // Start is called before the first frame update
    void Start()
    {
        if (!DetectorManager.Instance.useFileVideo)
        {
            // Webcam initialization
            webcamRaw = new WebCamTexture(DetectorManager.Instance.resolution.x, DetectorManager.Instance.resolution.y, 60);
            webcamBuffer = new RenderTexture(DetectorManager.Instance.resolution.x, DetectorManager.Instance.resolution.y, 0);
            readBuffer = new Color32[DetectorManager.Instance.resolution.x * DetectorManager.Instance.resolution.y];

            webcamRaw.Play();
            webcamPreview.texture = webcamBuffer;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!DetectorManager.Instance.useFileVideo)
        {
            // Webcam image buffering
            webcamRaw.GetPixels32(readBuffer);
            Graphics.Blit(webcamRaw, webcamBuffer);

            DetectorManager.Instance.UpdateApriltags(readBuffer);
        }
    }

    void OnDestroy()
    {
        Destroy(webcamRaw);
        Destroy(webcamBuffer);
    }
}
