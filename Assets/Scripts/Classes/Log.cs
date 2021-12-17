using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A log entry
/// </summary>
[System.Serializable]
public class Log
{
    /// <summary>
    /// Contructor for log
    /// </summary>
    /// <param name="_timestamp">The timestamp of the log</param>
    /// <param name="_relativeGazePositon">The position of the gaze</param>
    public Log(double _timestamp, Vector2 _relativeGazePositon)
    {
        Timestamp = _timestamp;
        RelativeGazePosition = _relativeGazePositon;
    }

    /// <summary>
    /// The timestamp of the log
    /// </summary>
    public double Timestamp;

    /// <summary>
    /// The position of the gaze
    /// </summary>
    public Vector2 RelativeGazePosition;
}
