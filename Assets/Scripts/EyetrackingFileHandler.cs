using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// File handler for the eyetracking data file by converting the JSON into an object
/// </summary>
public class EyetrackingFileHandler : MonoBehaviour
{
    /// <summary>
    /// Event which is triggered when the gaze position is updated
    /// </summary>
    public UnityEvent<Vector2> onGazePositionUpdated;

    /// <summary>
    /// The JSON-file with the gaze data
    /// </summary>
    [SerializeField] string gazeDataFile;

    /// <summary>
    /// List of the gaze datas
    /// </summary>
    [SerializeField] GazeDatas gazeDatas;

    /// <summary>
    /// The current position of the gaze
    /// </summary>
    [SerializeField] Vector2 gazePosition;

    /// <summary>
    /// The timestamp where the gazedata will be searched
    /// </summary>
    private double timestamp;

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        // Load configuration
        if (ConfigurationManager.Instance != null)
        {
            gazeDataFile = ConfigurationManager.Instance.gazedataPath;
        }

        // Prepare file to be JSON-processed (the file is not completely correct JSON)
        string json = System.IO.File.ReadAllText(gazeDataFile);
        json = "{\"gazedata\":[" + json + "]}";
        json = json.Replace("}\n{\"type", "}, {\"type");

        // Read JSON-file and convert to object
        gazeDatas = JsonUtility.FromJson<GazeDatas>(json);

        // Remove gaze data where no gaze data is available (gaze data is empty)
        for (int i = gazeDatas.gazedata.Count - 1; i >= 0; i--)
        {
            Gazedata gazeData = gazeDatas.gazedata[i];

            if (gazeData.data.gaze2d.Count == 0)
            {
                gazeDatas.gazedata.Remove(gazeData);
            }
        }
    }

    /// <summary>
    /// Set the timestamp where will be searched
    /// </summary>
    /// <param name="_timestamp">The timestamp</param>
    public void SetTimestamp(double _timestamp)
    {
        timestamp = _timestamp;
    }

    /// <summary>
    /// Search the gaze position using a timestamp and set it in the global variable 
    /// </summary>
    public void SearchGazePositionAtTimestamp()
    {
        // Find nearest gaze data
        Gazedata gazeData = GetNearestGazedata(timestamp);

        // Set gaze position
        gazePosition = new Vector2((float)gazeData.data.gaze2d[0], 1 - (float)gazeData.data.gaze2d[1]);

        // Trigger event
        onGazePositionUpdated?.Invoke(gazePosition);
    }

    /// <summary>
    /// Index to start looking for the gaze data for the next frame
    /// </summary>
    int currentIndex = 0;

    /// <summary>
    /// Get the nearest gaze data of a specific timestamp
    /// </summary>
    /// <param name="_timestamp">The timestamp</param>
    /// <returns>The gaze data</returns>
    public Gazedata GetNearestGazedata(double _timestamp)
    {
        // Initialize variables with values for first gaze data in gazeDatas
        float timeDifference = Mathf.Abs((float)(gazeDatas.gazedata[0].timestamp - _timestamp));
        Gazedata previousGazeData = gazeDatas.gazedata[0];

        // Go through the gaze data in gazeDatas, but start at where the gaze data for the previous frame was found. This optimizes the code a lot. We do not have to check the previous gaze datas, because the video is already past this point
        for (int i = currentIndex; i < gazeDatas.gazedata.Count - 1; i++)
        {
            // Get the gaze data at the specified index
            Gazedata gazeData = gazeDatas.gazedata[i];

            // Check if the new found time is bigger than the last found time. If so, the best time has already been found and we can stop looking for a better frame
            if (timeDifference < Mathf.Abs((float)(gazeData.timestamp - _timestamp)))
            {
                break;
            }

            // Update the current index
            currentIndex = i;

            // Save the gaze data for the next iteration
            previousGazeData = gazeData;

            // Update the time difference 
            timeDifference = Mathf.Abs((float)(gazeData.timestamp - _timestamp));
        }

        // Return the gaze data
        return previousGazeData;
    }

    /// <summary>
    /// Drawing of gizmos
    /// </summary>
    private void OnDrawGizmos()
    {
        // Check if there's a gaze position that can be drawn and if there is a camera
        if (gazePosition != null && Camera.main != null)
        {
            // Draw a red sphere at gaze position. To do this, the gaze position coordinates (which are in 2D and have (0, 0) in the top left corner) are converted to their respective coordinates in world space (which are in 3D and have (0, 0) in the center) using the camera dimensions and canvas scale.
            Gizmos.color = Color.red;
            GameObject canvas = Object.FindObjectOfType<Canvas>().gameObject;
            Gizmos.DrawSphere(new Vector3((gazePosition.x * Camera.main.pixelWidth - Camera.main.pixelWidth / 2) * canvas.transform.localScale.x, (gazePosition.y * Camera.main.pixelHeight - Camera.main.pixelHeight / 2) * canvas.transform.localScale.y, 1000), 15);
        }
    }
}
