using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DigitalApriltag
{
    /// <summary>
    /// Constructor for digital Apriltag
    /// </summary>
    /// <param name="_ID">The ID of the Apriltag</param>
    /// <param name="_offset">Position of Apriltag relative to center of table</param>
    public DigitalApriltag(int _ID, Vector3 _offset)
    {
        ID = _ID;
        Offset = _offset;
        Color = Color.red;
    }

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
