using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawTable : MonoBehaviour
{
    /// <summary>
    /// List of all digital Apriltags
    /// </summary>
    public List<DigitalApriltag> digitalApriltags;

    /// <summary>
    /// Rotation offset when comparing Apriltag and table rotation
    /// </summary>
    public Vector3 rotationOffset;

    /// <summary>
    /// Scale for the table. Lower numbers make the table appear close (thus larger).
    /// </summary>
    public float scaleCalibration = 50f;
    
    // Start is called before the first frame update
    void Start()
    {
        AprilTag.TagDetector detector = new AprilTag.TagDetector(1920, 1080);
    }

    // Update is called once per frame
    public void UpdateTable(List<AprilTag.TagPose> apriltags)
    {
        // Get the physical Apriltag position
        AprilTag.TagPose apriltag = new AprilTag.TagPose();

        // Rotate the table in the same way as the first physical Apriltag
        if (apriltags.Count > 0)
        {
            apriltag = apriltags[0];
            transform.rotation = apriltag.Rotation * Quaternion.Euler(rotationOffset);
        }
        
        // Position the table at the same correct position, according to the first physical Apriltag
        Vector3 currentPosition = RelativeVector(digitalApriltags[0].Position);
        transform.position = transform.position + apriltag.Position * scaleCalibration - currentPosition;
    }
    
    /// <summary>
    /// Return absolute position of a Vector3, relative to origin, rotation, scale and position of plane
    /// </summary>
    /// <param name="_vector3">Relative position</param>
    /// <returns>The absolute Vector3</returns>
    Vector3 RelativeVector(Vector3 _vector3)
    {
        // Create new Vector3
        Vector3 relativeVector = transform.position;

        // Calculate relative position
        relativeVector += transform.right * _vector3.x * transform.localScale.x;
        relativeVector += transform.up * _vector3.y * transform.localScale.y;
        relativeVector += transform.forward * _vector3.z * transform.localScale.z;

        // Return new vector
        return relativeVector;
    }

    // Drawing of Gizmos
    void OnDrawGizmos()
    {
        foreach (DigitalApriltag digitalApriltag in digitalApriltags)
        {
            Gizmos.color = digitalApriltag.Color;
            Gizmos.DrawSphere(RelativeVector(digitalApriltag.Position), 0.5f);
        }
    }
}
