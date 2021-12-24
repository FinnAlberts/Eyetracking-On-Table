using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Detect Apriltags in a given frame
/// </summary>
public class DetectorManager : MonoBehaviour
{
    /// <summary>
    /// Event for when detected Apriltags have been updated
    /// </summary>
    public UnityEvent<List<AprilTag.TagPose>> onDetectedApriltagsUpdated;

    /// <summary>
    /// Event for when no Apriltags have been detected
    /// </summary>
    public UnityEvent onNoApriltagsDetected;

    /// <summary>
    /// Choose if webcam or a set video file (in video player) should be used
    /// </summary>
    public bool useFileVideo = false;

    /// <summary>
    /// Resolution of input file (can be video or webcam)
    /// </summary>
    public Vector2Int resolution = new Vector2Int(1920, 1080); 

    /// <summary>
    /// Decimation for Apriltag processing. When using higher values, accuracy may drop, but preformance will increase
    /// </summary>
    [SerializeField] int decimation = 1;

    /// <summary>
    /// Size of the Apriltags in meters
    /// </summary>
    [SerializeField] float tagSize = 0.05f;

    /// <summary>
    /// The Apriltag detector
    /// </summary>
    private AprilTag.TagDetector detector;

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

    /// <summary>
    /// Called before the first frame update
    /// </summary>
    private void Start()
    {
        // Load configuration
        if (ConfigurationManager.Instance != null)
        {
            tagSize = ConfigurationManager.Instance.tagSize;
            decimation = ConfigurationManager.Instance.decimation;
        }

        // Switch between video file and webcam
        if (useFileVideo)
        {
            Object.FindObjectOfType<WebcamHandler>().gameObject.SetActive(false);
            Object.FindObjectOfType<VideoFileHandler>().gameObject.SetActive(true);
        }
        else
        {
            Object.FindObjectOfType<WebcamHandler>().gameObject.SetActive(true);
            Object.FindObjectOfType<VideoFileHandler>().gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Initialize the Apriltag detector using the values set in the global variables
    /// </summary>
    public void Initialize()
    {
        // Detector initialization
        detector = new AprilTag.TagDetector(resolution.x, resolution.y, decimation);
    }

    /// <summary>
    /// Updates the found Apriltags using a given frame
    /// </summary>
    /// <param name="_frame">The frame to detect the Apriltags in</param>
    public void UpdateApriltags(Color32[] _frame)
    {
        // Check if detector has been initialized
        if (detector == null)
        {
            return;
        }        
        
        // Apriltag detection
        var fov = Camera.main.fieldOfView * Mathf.Deg2Rad;
        detector.ProcessImage(_frame, fov, tagSize);

        // Set detected Apriltags in list
        List<AprilTag.TagPose> apriltags = detector.DetectedTags.ToList();

        DetectorManager.Instance.detectingTime= Time.realtimeSinceStartupAsDouble - DetectorManager.Instance.startTime;
        DetectorManager.Instance.startTime = Time.realtimeSinceStartupAsDouble;

        // Check if Apriltags have been detected and trigger corresponding event
        if (apriltags.Count == 0)
        {
            onNoApriltagsDetected?.Invoke();
        } else
        {
            onDetectedApriltagsUpdated?.Invoke(apriltags);
        }
    }

    /// <summary>
    /// Called on destroy
    /// </summary>
    void OnDestroy()
    {
        detector.Dispose();
    }

    // DEBUG VARIABLES
    // TODO: remove this!!!!!!
    public double startTime = 0;
    public double frame = 0;
    public double to2DTime = 0;
    public double detectingTime = 0;
    public double tableTime = 0;
    public double searchGazeTime = 0;
    public double raycastTime = 0;
}
