using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data for one eye
/// </summary>
[System.Serializable]
public class Eye
{
    /// <summary>
    /// Origin of gaze
    /// </summary>
    public List<double> gazeorigin;

    /// <summary>
    /// Direction of gaze
    /// </summary>
    public List<double> gazedirection;

    /// <summary>
    /// Diameter of pupil
    /// </summary>
    public double pupildiameter;
}

/// <summary>
/// Data of a gaze
/// </summary>
[System.Serializable]
public class Data
{
    /// <summary>
    /// The gaze in 2D (values between 0 and 1, (0, 0) is top left corner)
    /// </summary>
    public List<double> gaze2d;

    /// <summary>
    /// The gaze in 3D
    /// </summary>
    public List<double> gaze3d;

    /// <summary>
    /// The left eye
    /// </summary>
    public Eye eyeleft;

    /// <summary>
    /// The right eye
    /// </summary>
    public Eye eyeright;
}

/// <summary>
/// Data of a gaze including time and timestamp
/// </summary>
[System.Serializable]
public class Gazedata
{
    /// <summary>
    /// The type of the JSON-object
    /// </summary>
    public string type;

    /// <summary>
    /// The timestamp, measured from the moment the recording started
    /// </summary>
    public double timestamp;

    /// <summary>
    /// The data
    /// </summary>
    public Data data;
}

/// <summary>
/// Parent of all gaze data
/// </summary>
[System.Serializable]
public class GazeDatas
{
    /// <summary>
    /// List of all gaze data
    /// </summary>
    public List<Gazedata> gazedata;
}
