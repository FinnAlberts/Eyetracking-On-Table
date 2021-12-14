using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Log a Vector2 to a specified file
/// </summary>
public class Logger : MonoBehaviour
{
    /// <summary>
    /// File to log to
    /// </summary>
    [SerializeField] string outputFile;

    /// <summary>
    /// Log a Vector2 to a log file
    /// </summary>
    /// <param name="_vector2">The Vector2 to be logged </param>
    public void LogVector(Vector2 _vector2)
    {
        // Format string including DateTime
        DateTime dateTime = DateTime.Now;
        string location = (Mathf.Round(_vector2.x * 100) / 100).ToString() + " " + (Mathf.Round(_vector2.y * 100) / 100).ToString();
        string text = String.Format("[{0}] {1}\n", "", location);

        // Add text to file
        File.AppendAllText(outputFile, text);
    }
}
