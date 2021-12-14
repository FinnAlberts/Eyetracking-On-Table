using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DigitalApriltag
{
    /// <summary>
    /// The ID of the Apriltag
    /// </summary>
    public int ID;

    /// <summary>
    /// Position of Apriltag relative to center of table
    /// </summary>
    public Vector3 Offset;

    /// <summary>
    /// Color of the Apriltag
    /// </summary>
    public Color Color;
}
