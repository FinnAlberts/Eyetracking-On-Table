using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class DetectorManager : MonoBehaviour
{
    /// <summary>
    /// Choose if webcam or a set video file (in video player) should be used
    /// </summary>
    public bool useFileVideo = false;

    /// <summary>
    /// Event for when detected Apriltags have been updated
    /// </summary>
    public UnityEvent<List<AprilTag.TagPose>> onDetectedApriltagsUpdated;

    // AprilTag variables
    public Vector2Int resolution = new Vector2Int(1920, 1080);
    public int decimation = 1;
    public float tagSize = 0.05f;
    AprilTag.TagDetector detector;

    /// <summary>
    /// The camera
    /// </summary>
    public Camera cam;

    #region Make class into a singleton
    private static DetectorManager _instance;

    public static DetectorManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    #endregion

    // Called before first frame
    private void Start()
    {
        // Detector initialization
        detector = new AprilTag.TagDetector(resolution.x, resolution.y, decimation);

        // Enable/disable webcam preview
        if (useFileVideo)
        {
            GameObject.Find("Canvas").SetActive(false);
        }
        else
        {
            GameObject.Find("Canvas").SetActive(true);
        }
    }

    /// <summary>
    /// Updates the found AprilTags using a given frame
    /// </summary>
    /// <param name="_frame">The frame to detect the AprilTags in</param>
    public void UpdateApriltags(Color32[] _frame)
    {
        // AprilTag detection
        var fov = cam.fieldOfView * Mathf.Deg2Rad;
        detector.ProcessImage(_frame, fov, tagSize);

        // Trigger event with list of currently detected Apriltags
        List<AprilTag.TagPose> apriltags = detector.DetectedTags.ToList();
        onDetectedApriltagsUpdated?.Invoke(apriltags);
    }

    void OnDestroy()
    {
        detector.Dispose();
    }
}
