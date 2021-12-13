using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

public class EyetrackingDataProcessor : MonoBehaviour
{
    /// <summary>
    /// The JSON-file with the gaze data
    /// </summary>
    public string GazeDataFile;

    /// <summary>
    /// List of the gaze datas
    /// </summary>
    public GazeDatas gazeDatas;

    /// <summary>
    /// The current position of the gaze
    /// </summary>
    public Vector2 gazePosition;

    /// <summary>
    /// Event which is trigger when the data for 1 frame is processed
    /// </summary>
    public UnityEvent onDataProcessingComplete;

    /// <summary>
    /// The canvas (used for scaling)
    /// </summary>
    public Canvas canvas;

    /// <summary>
    /// Event which is triggered when the gaze position is updated
    /// </summary>
    public UnityEvent<Vector2> onGazePositionUpdated;

    // Start is called before the first frame update
    void Start()
    {
        // Prepare file to be JSON-processed (the file is not completely correct JSON)
        string json = System.IO.File.ReadAllText(GazeDataFile);
        json = "{\"gazedata\":[" + json + "]}";
        json = json.Replace("}\n{\"type", "}, {\"type");

        // Read JSON-file and convert to object
        gazeDatas = JsonUtility.FromJson<GazeDatas>(json);

        // Remove gaze data where no gaze data is available (gaze data is empty)
        for (int i = gazeDatas.gazedata.Count - 1; i > 0; i--)
        {
            Gazedata gazeData = gazeDatas.gazedata[i];

            if (gazeData.data.gaze2d.Count == 0)
            {
                gazeDatas.gazedata.Remove(gazeData);
            }
        }
    }

    /// <summary>
    /// Set the gaze position in the global variable using the according timestamp
    /// </summary>
    /// <param name="_timestamp">The timestamp at which the gaze needs to be determined and set</param>
    public void ProjectGaze2DUsingTime(double _timestamp)
    {
        // Find nearest gaze data
        Gazedata gazeData = GetNearestGazedata(_timestamp);

        // Set gaze position
        float gazePositionX = ((float)gazeData.data.gaze2d[0] * Camera.main.pixelWidth - Camera.main.pixelWidth / 2) * canvas.transform.localScale.x;
        float gazePositionY = ((1 - (float)gazeData.data.gaze2d[1]) * Camera.main.pixelHeight - Camera.main.pixelHeight / 2) * canvas.transform.localScale.y;
        gazePosition = new Vector2(gazePositionX, gazePositionY);

        // Resume video
        onDataProcessingComplete?.Invoke();
        onGazePositionUpdated?.Invoke(new Vector2((float)gazeData.data.gaze2d[0], (1 - (float)gazeData.data.gaze2d[1])));
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

    private void OnDrawGizmos()
    {
        if (gazePosition != null && DetectorManager.Instance != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(new Vector3(gazePosition.x, gazePosition.y, 1000), 15);
        }
    }
}
