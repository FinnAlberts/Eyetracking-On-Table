using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Table : MonoBehaviour
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

    /// <summary>
    /// List of physical Apriltags
    /// </summary>
    private List<AprilTag.TagPose> apriltags = new List<AprilTag.TagPose>();

    // Start is called before the first frame update
    void Start()
    {
        AprilTag.TagDetector detector = new AprilTag.TagDetector(1920, 1080);
    }

    // Runs every time the detected Apriltags are updated
    public void UpdateTable(List<AprilTag.TagPose> _apriltags)
    {
        apriltags = _apriltags;

        #region Calculate rotation
        /*List<Vector3> rotations = new List<Vector3>();
        foreach (AprilTag.TagPose apriltag in apriltags)
        {
            rotations.Add(apriltag.Rotation.eulerAngles);
        }

        if (rotations.Count == 0)
        {
            return;
        }

        Vector3 medianEulerRotation = MedianVector3(rotations);
        GetRotationUsingPosition(_apriltags, digitalApriltags);

        transform.rotation = Quaternion.Euler(medianEulerRotation) * Quaternion.Euler(rotationOffset);*/

        transform.rotation = GetRotationUsingPosition(_apriltags, digitalApriltags) * Quaternion.Euler(rotationOffset);
        #endregion

        #region Calculate position
        Vector3 originPosition = Vector3.zero;

        // Foreach found Apriltag, calculate origin of table
        foreach (AprilTag.TagPose apriltag in apriltags)
        {
            DigitalApriltag digitalApriltag = digitalApriltags.Find(a => a.ID == apriltag.ID);

            if (digitalApriltag != null)
            {
                originPosition += GetOriginUsingApriltag(digitalApriltag, apriltag);
            }
        }

        // Calculate average origin
        originPosition /= apriltags.Count;

        // Move table to that origin
        transform.position = originPosition;
        #endregion
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

    /// <summary>
    /// Calculate the origin of the table based on a detected Apriltag and its representing digital Apriltag
    /// </summary>
    /// <param name="_digitalApriltag">The digital Apriltag</param>
    /// <param name="_apriltag">The pysical Apriltag</param>
    /// <returns>The origin of the table</returns>
    Vector3 GetOriginUsingApriltag(DigitalApriltag _digitalApriltag, AprilTag.TagPose _apriltag)
    {
        // Create new Vector3 for position and offset
        Vector3 position = _apriltag.Position * scaleCalibration;
        Vector3 offset = _digitalApriltag.Position;

        // Calculate origin
        position -= transform.right * offset.x * transform.localScale.x;
        position -= transform.up * offset.y * transform.localScale.y;
        position -= transform.forward * offset.z * transform.localScale.z;

        // Return origin
        return position;
    }

    /// <summary>
    /// Calculate the median of a list Vector3s, calculating x, y and z all seperately.
    /// </summary>
    /// <param name="_vector3s">List of Vector3s</param>
    /// <returns>The median Vector3</returns>
    Vector3 MedianVector3(List<Vector3> _vector3s)
    {
        // Create a list for values of x, y and z and order them
        List<float> x = _vector3s.Select(v => v.x).OrderBy(v => v).ToList();
        List<float> y = _vector3s.Select(v => v.y).OrderBy(v => v).ToList();
        List<float> z = _vector3s.Select(v => v.z).OrderBy(v => v).ToList();

        // Calculate median values. If there are 2 values 'in the middle', use the first one.
        float medianX = x[(int)Mathf.Floor(x.Count / 2)];
        float medianY = y[(int)Mathf.Floor(y.Count / 2)];
        float medianZ = z[(int)Mathf.Floor(z.Count / 2)];

        // Return a new Vector3 with the median x, y and z
        return new Vector3(medianX, medianY, medianZ);
    }

    Quaternion GetRotationUsingPosition(List<AprilTag.TagPose> _apriltags, List<DigitalApriltag> _digitalApriltags)
    {
        // Check if list has at least 3 tags
        if (_apriltags.Count < 3)
        {
            return Quaternion.Euler(Vector3.zero);
        }
        
        // Get a list of all digital Apriltags that have been detected
        List<DigitalApriltag> foundDigitalApriltags = _digitalApriltags.Where(d => (_digitalApriltags.Select(d => d.ID).Intersect(_apriltags.Select(a => a.ID)).Contains(d.ID))).ToList();

        // Initalize a list for candidates for the three digital Apriltags: upper left, lower left and upper right. Tags have to be unique.
        List<DigitalApriltag> candidates = new List<DigitalApriltag>();

        // Find most upper left digital Apriltag, where up is more important than left
        candidates = foundDigitalApriltags.Where(d => d.Position.z == foundDigitalApriltags.Max(a => a.Position.z)).ToList();
        DigitalApriltag upperLeftDigital = candidates.Where(d => d.Position.x == candidates.Min(a => a.Position.x)).FirstOrDefault();

        // Remove the found digital Apriltag from the list
        foundDigitalApriltags.Remove(upperLeftDigital);

        // Find most upper right digital Apriltag, where right is more important than up
        candidates = foundDigitalApriltags.Where(d => d.Position.x == foundDigitalApriltags.Max(a => a.Position.x)).ToList();
        DigitalApriltag upperRightDigital = candidates.Where(d => d.Position.z == candidates.Max(a => a.Position.z)).FirstOrDefault();

        // Remove the found digital Apriltag from the list
        foundDigitalApriltags.Remove(upperRightDigital);

        // Find most lower left digital Apriltag, where left is more important than down.
        candidates = foundDigitalApriltags.Where(d => d.Position.z == foundDigitalApriltags.Min(a => a.Position.z)).ToList();
        DigitalApriltag lowerLeftDigital = candidates.Where(d => d.Position.x == candidates.Min(a => a.Position.x)).FirstOrDefault();

        // TEMP: debug the digital tag positions
        Debug.Log("ul " + upperLeftDigital.Position + " ur " + upperRightDigital.Position + " ll " + lowerLeftDigital.Position);

        // Check if Apriltags are on a straight line, in that case, complete rotation cannot be determined
        if (
            (upperLeftDigital.Position.x == upperRightDigital.Position.x) && (upperLeftDigital.Position.x == lowerLeftDigital.Position.x) ||
            (upperLeftDigital.Position.z == upperRightDigital.Position.z) && (upperLeftDigital.Position.z == lowerLeftDigital.Position.z))
        {
            Debug.Log("Tags are on a single line");
            return Quaternion.Euler(Vector3.zero);
        }

        // Get physical Apriltags corresponding to the found digital Apriltags
        Vector3 upperLeftPosition = _apriltags.Where(a => a.ID == upperLeftDigital.ID).FirstOrDefault().Position;
        Vector3 upperRightPosition = _apriltags.Where(a => a.ID == upperRightDigital.ID).FirstOrDefault().Position;
        Vector3 lowerLeftPosition = _apriltags.Where(a => a.ID == lowerLeftDigital.ID).FirstOrDefault().Position;

        // Get x-rotation
        Vector2 upperLeftYZ = new Vector2(upperLeftPosition.y, upperLeftPosition.z);
        Vector2 lowerLeftYZ = new Vector2(lowerLeftPosition.y, lowerLeftPosition.z);

        Vector2 xDirection = lowerLeftYZ - upperLeftYZ;

        float xAngle = Vector2.Angle(Vector2.up, xDirection);

        // Get y-rotation
        Vector2 upperLeftXZ = new Vector2(upperLeftPosition.x, upperLeftPosition.z);
        Vector2 upperRightXZ = new Vector2(upperRightPosition.x, upperRightPosition.z);

        Vector2 yDirection = upperLeftXZ - upperRightXZ;

        float yAngle = Vector2.Angle(Vector2.down, yDirection);

        // Get z-rotation
        Vector2 upperLeftXY = new Vector2(upperLeftPosition.x, upperLeftPosition.y);
        Vector2 upperRightXY = new Vector2(upperRightPosition.x, upperRightPosition.y);

        Vector2 zDirection = upperRightXY - upperLeftXY;

        float zAngle = Vector2.Angle(Vector2.right, zDirection);

        Debug.Log(xAngle + " | " + yAngle + " | " + zAngle);

        //return new Vector3(xAngle, yAngle, zAngle);
        return Quaternion.Euler(xAngle,yAngle,zAngle);
    }

    // Drawing of Gizmos
    void OnDrawGizmos()
    {
        // Draw points of digital Apriltags
        foreach (DigitalApriltag digitalApriltag in digitalApriltags)
        {
            Gizmos.color = digitalApriltag.Color;
            Gizmos.DrawSphere(RelativeVector(digitalApriltag.Position), 0.5f);
        }

        // Draw rays from Apriltags
        Gizmos.color = Color.white;
        foreach (AprilTag.TagPose apriltag in apriltags)
        {
            Gizmos.DrawRay(apriltag.Position, apriltag.Rotation * Vector3.back);
        }
    }
}
