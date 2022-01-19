using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains the configuration for the processing
/// </summary>
public class ConfigurationManager : MonoBehaviour
{
    /// <summary>
    /// Path to video
    /// </summary>
    public string videoPath;

    /// <summary>
    /// Path to gaze data file
    /// </summary>
    public string gazedataPath;

    /// <summary>
    /// Scale of the table
    /// </summary>
    public Vector2 tableScale;

    /// <summary>
    /// Scale calibrator
    /// </summary>
    public float scaleCalibration;

    /// <summary>
    /// Size of Apriltags in centimeters
    /// </summary>
    public float tagSize;

    /// <summary>
    /// List of digital Apriltags
    /// </summary>
    public List<DigitalApriltag> digitalApriltags;

    /// <summary>
    /// Decimation for Apriltag processing
    /// </summary>
    public int decimation;

    /// <summary>
    /// Path for output file
    /// </summary>
    public string outputPath;

    #region Make class into a DontDestroyOnLoad singleton
    private static ConfigurationManager _instance;

    public static ConfigurationManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    #endregion 
}
