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
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LogVector(Vector2 _vector2)
    {
        DateTime dateTime = DateTime.Now;
        string location = _vector2.x.ToString() + " " + _vector2.y.ToString();
        string text = String.Format("[{0}] {1}\n", dateTime, location);
        File.AppendAllText(outputFile, text);
    }
}
