using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UI = UnityEngine.UI;

public class ApriltagDetector : MonoBehaviour
{
    public UnityEvent<List<AprilTag.TagPose>> onDetectedApriltagsUpdated;

    [SerializeField] Vector2Int _resolution = new Vector2Int(1920, 1080);
    [SerializeField] int _decimation = 1;
    [SerializeField] float _tagSize = 0.05f;
    [SerializeField] UI.RawImage _webcamPreview = null;

    WebCamTexture _webcamRaw;
    RenderTexture _webcamBuffer;
    Color32[] _readBuffer;

    AprilTag.TagDetector _detector;

    // Start is called before the first frame update
    void Start()
    {
        // Webcam initialization
        _webcamRaw = new WebCamTexture(_resolution.x, _resolution.y, 60);
        _webcamBuffer = new RenderTexture(_resolution.x, _resolution.y, 0);
        _readBuffer = new Color32[_resolution.x * _resolution.y];

        _webcamRaw.Play();
        _webcamPreview.texture = _webcamBuffer;

        // Detector and drawer
        _detector = new AprilTag.TagDetector(_resolution.x, _resolution.y, _decimation);
    }

    // Update is called once per frame
    void Update()
    {
        // Webcam image buffering
        _webcamRaw.GetPixels32(_readBuffer);
        Graphics.Blit(_webcamRaw, _webcamBuffer);

        // AprilTag detection
        var fov = GetComponent<Camera>().fieldOfView * Mathf.Deg2Rad;
        _detector.ProcessImage(_readBuffer, fov, _tagSize);

        // Trigger event with list of currently detected Apriltags
        List<AprilTag.TagPose> apriltags = _detector.DetectedTags.ToList();
        onDetectedApriltagsUpdated?.Invoke(apriltags);
    }

    void OnDestroy()
    {
        Destroy(_webcamRaw);
        Destroy(_webcamBuffer);

        _detector.Dispose();
    }
}
