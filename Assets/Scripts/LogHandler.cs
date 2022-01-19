using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Log a Vector2 to a specified file
/// </summary>
public class LogHandler : MonoBehaviour
{
    /// <summary>
    /// File to log to
    /// </summary>
    [SerializeField] string outputFile;

    /// <summary>
    /// The timestamp where the gazedata will be searched
    /// </summary>
    private double timestamp;

    private void Start()
    {
        // Load configuration
        if (ConfigurationManager.Instance != null)
        {
            outputFile = ConfigurationManager.Instance.outputPath;
        }
    }

    /// <summary>
    /// Set the timestamp for the log
    /// </summary>
    /// <param name="_timestamp">The timestamp</param>
    public void SetTimestamp(double _timestamp)
    {
        timestamp = _timestamp;
    }

    /// <summary>
    /// Log a Vector2 to a log file. The Vector2 should contain values between -5 and 5. These will be converted to centimeters using the dimensions on the table.
    /// </summary>
    /// <param name="_vector2">The Vector2 to be logged</param>
    public void LogVector(Vector2 _vector2)
    {
        // Convert to centimeters on table
        float x = _vector2.x;
        float y = _vector2.y;

        x = (x + 5) / 10;
        y = (y + 5) / 10;

        float xInCentimeters = ConfigurationManager.Instance.tableScale.x * 1000 * x;
        float yInCentimeters = ConfigurationManager.Instance.tableScale.y * 1000 * y;

        Vector2 positionInCentimeters = new Vector2(xInCentimeters, yInCentimeters);

        // Format JSON
        Log log = new Log(timestamp, positionInCentimeters);
        string json = JsonUtility.ToJson(log);

        // Add text to file
        File.AppendAllText(outputFile, String.Format("{0}\n", json));
    }
}
