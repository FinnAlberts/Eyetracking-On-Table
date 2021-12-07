using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Logger : MonoBehaviour
{
    /// <summary>
    /// File to log to
    /// </summary>
    public string outputFile;

    public void LogVector(Vector2 _vector2)
    {
        DateTime dateTime = DateTime.Now;
        string location = _vector2.x.ToString() + " " + _vector2.y.ToString();
        string text = String.Format("[{0}] {1}\n", dateTime, location);
        File.AppendAllText(outputFile, text);
    }
}
