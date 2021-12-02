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

    /// <summary>
    /// Upwards direction, lerped
    /// </summary>
    private Vector3 upwardDirectionLerped = Vector3.zero;

    /// <summary>
    /// Forward direction, lerped
    /// </summary>
    private Vector3 forwardDirectionLerped = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize detector
        AprilTag.TagDetector detector = new AprilTag.TagDetector(1920, 1080);
    }

    // Runs every time the detected Apriltags are updated
    public void UpdateTable(List<AprilTag.TagPose> _apriltags)
    {
        // Set detected tags in global list
        apriltags = _apriltags;

        // Set rotation of table using rotation of Apriltags
        SetRotationUsingTagRotation();

        // Set position of table
        SetPosition();
    }

    /// <summary>
    /// Set the position of the table
    /// </summary>
    void SetPosition()
    {
        // Check if Apriltags have been detected
        if (apriltags.Count == 0)
        {
            return;
        }

        // Initialize variable for new position
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
    }

    /// <summary>
    /// Return absolute position of a Vector3, relative to origin, rotation, scale and position of plane
    /// </summary>
    /// <param name="_vector3">Relative position</param>
    /// <returns>The absolute Vector3</returns>
    Vector3 GetAbsolutePosition(Vector3 _vector3)
    {
        // Create new Vector3
        Vector3 absolutePosition = transform.position;

        // Calculate relative position
        absolutePosition += transform.right * _vector3.x * transform.localScale.x;
        absolutePosition += transform.up * _vector3.y * transform.localScale.y;
        absolutePosition += transform.forward * _vector3.z * transform.localScale.z;

        // Return new vector
        return absolutePosition;
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
    /// Set the rotation of the table based on positions of Apriltags, or if not enough tags have been detected, the rotation of tha detected Apriltags themselves
    /// </summary>
    void SetRotation()
    {
        // Check if Apriltags have been detected
        if (apriltags.Count == 0)
        {
            return;
        }
        
        // Check if list has at least 3 tags
        if (apriltags.Count < 3)
        {
            Debug.Log("Not enough visible tags to determine rotation");
            SetRotationUsingTagRotation();
            return;
        }

        #region Find upper left, upper right and lower left digital apriltag
        // Get a list of all digital Apriltags that have been detected
        List<DigitalApriltag> foundDigitalApriltags = digitalApriltags.Where(d => (digitalApriltags.Select(d => d.ID).Intersect(apriltags.Select(a => a.ID)).Contains(d.ID))).ToList();

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
        #endregion

        // Check if Apriltags are on a straight line, in that case, complete rotation cannot be determined
        if (
            (upperLeftDigital.Position.x == upperRightDigital.Position.x) && (upperLeftDigital.Position.x == lowerLeftDigital.Position.x) ||
            (upperLeftDigital.Position.z == upperRightDigital.Position.z) && (upperLeftDigital.Position.z == lowerLeftDigital.Position.z))
        {
            Debug.Log("Tags are on a single line");
            SetRotationUsingTagRotation();
            return;
        }

        // Get physical Apriltags corresponding to the found digital Apriltags
        Vector3 upperLeftPosition = apriltags.Where(a => a.ID == upperLeftDigital.ID).FirstOrDefault().Position;
        Vector3 upperRightPosition = apriltags.Where(a => a.ID == upperRightDigital.ID).FirstOrDefault().Position;
        Vector3 lowerLeftPosition = apriltags.Where(a => a.ID == lowerLeftDigital.ID).FirstOrDefault().Position;

        // Draw rays to show which Apriltags are being used to determine rotation
        Debug.DrawRay(upperLeftPosition, transform.up, Color.red, Time.deltaTime);
        Debug.DrawRay(upperRightPosition, transform.up, Color.blue, Time.deltaTime);
        Debug.DrawRay(lowerLeftPosition, transform.up, Color.green, Time.deltaTime);

        // Calculate the normal of the plane and apply this normal to the upper direction of the table plane
        Vector3 normal = Vector3.Cross(upperRightPosition - upperLeftPosition, lowerLeftPosition - upperLeftPosition);
        transform.up = normal;

        // Calculate offset angle 
        Vector2 upperLeftLocal = new Vector2(upperLeftDigital.Position.x * transform.localScale.x, upperLeftDigital.Position.z * transform.localScale.z);
        Vector2 lowerLeftLocal = new Vector2(lowerLeftDigital.Position.x * transform.localScale.x, lowerLeftDigital.Position.z * transform.localScale.z);

        float offsetAngle = GetAngle(upperLeftLocal, lowerLeftLocal, Vector2.down, -Vector2.right);

        // Project upper left and lower left to the normal plane
        Vector3 upperLeftProjected = Vector3.ProjectOnPlane(upperLeftPosition, normal);
        Vector3 lowerLeftProjected = Vector3.ProjectOnPlane(lowerLeftPosition, normal);

        // Calculate the forward direction of the plane
        Vector3 directionForward = GetDirection(upperLeftProjected, lowerLeftProjected).normalized;

        // Set the plane rotation to the forward direction and upward (= normal) direction
        transform.rotation = Quaternion.LookRotation(directionForward, normal);

        // Rotate the object in the calculated offset angle
        transform.Rotate(Vector3.up, 180 + offsetAngle);
    }

    /// <summary>
    /// Set the rotation based on rotation of the Apriltags
    /// </summary>
    void SetRotationUsingTagRotation()
    {
        // Initalize two variables for the average upwards direction and the average forward directions
        Vector3 averageUpwardDirection = Vector3.zero;
        Vector3 averageForwardDirection = Vector3.zero;

        // Use every Apriltag to dermine the average rotation
        foreach (AprilTag.TagPose apriltag in apriltags)
        {
            // Split the rotation in a forward direction and an upward direction
            Vector3 upwardDirection = apriltag.Rotation * Vector3.back;
            Vector3 forwardDirection = apriltag.Rotation * Vector3.up;

            // To correct for flickering, because the detector sometimes thinks the Apriltag is upside down, check if an Apriltag is facing down and if so, flip it around.
            // This will not cause issues, because the user will not look at the table upside down, so an Apriltag is always supposed to be looking up.
            if (Vector3.Dot(Vector3.up, upwardDirection) < 0)
            {
                upwardDirection = -upwardDirection;
            }

            // Add the directions to the average
            averageUpwardDirection += upwardDirection;
            averageForwardDirection += forwardDirection;
        }

        // Check if Apriltags have been detected. If not, return.
        if (averageForwardDirection == Vector3.zero && averageUpwardDirection == Vector3.zero)
        {
            return;
        }

        // Use a lerped direction for smooth rotations
        forwardDirectionLerped = Vector3.Lerp(forwardDirectionLerped, averageForwardDirection, Time.deltaTime * 10);
        upwardDirectionLerped = Vector3.Lerp(upwardDirectionLerped, averageUpwardDirection, Time.deltaTime * 10);

        // Set the rotation while applying the set offset
        transform.rotation = Quaternion.LookRotation(forwardDirectionLerped, upwardDirectionLerped) * Quaternion.Euler(rotationOffset);
    }

    /// <summary>
    /// Calculate the direction between 2 points.
    /// </summary>
    /// <param name="_pointA">Point from</param>
    /// <param name="_pointB">Point to</param>
    /// <returns></returns>
    public Vector3 GetDirection(Vector3 _pointA, Vector3 _pointB)
    {
        return (_pointB - _pointA).normalized;
    }

    /// <summary>
    /// Calculate the angle between two points
    /// </summary>
    /// <param name="_positionA">Position 1</param>
    /// <param name="_positionB">Position 2</param>
    /// <param name="_fromDirection">Direction to calculate angle from.</param>
    /// <param name="_dotDirection">The direction used to calculate if the angle is left or right from the fromDirection.</param>
    /// <returns></returns>
    public float GetAngle(Vector2 _positionA, Vector2 _positionB, Vector2 _fromDirection, Vector2 _dotDirection)
    {
        // Get direction between the two points
        Vector2 direction = GetDirection(_positionA, _positionB);

        // Calculate angle and dot
        float angle = Vector2.Angle(_fromDirection, direction);
        float dot = Vector2.Dot(direction, -_dotDirection);

        // Check if angle is left or right from fromDirection
        if (dot < 0)
        {
            // Apply correction
            angle = -angle;
        }

        // Return the angle
        return angle;
    }

    // Drawing of Gizmos
    void OnDrawGizmos()
    {
        // Draw points of digital Apriltags
        foreach (DigitalApriltag digitalApriltag in digitalApriltags)
        {
            Gizmos.color = digitalApriltag.Color;
            Gizmos.DrawSphere(GetAbsolutePosition(digitalApriltag.Position), 0.005f);
        }

        // Draw rays from Apriltags
        foreach (AprilTag.TagPose apriltag in apriltags)
        {
            // Split the rotation in an upward direction and a forward direction
            Vector3 upwardDirection = apriltag.Rotation * Vector3.back;
            Vector3 forwardDirection = apriltag.Rotation * Vector3.up;

            // To correct for flickering, because the detector sometimes thinks the Apriltag is upside down, check if an Apriltag is facing down and if so, flip it around.
            // This will not cause issues, because the user will not look at the table upside down, so an Apriltag is always supposed to be looking up.
            if (Vector3.Dot(Vector3.up, upwardDirection) < -0.2f)
            {
                upwardDirection = -upwardDirection;
            }

            // Draw rotation of Apriltags
            Gizmos.color = Color.white;
            Gizmos.DrawRay(apriltag.Position, upwardDirection);
            Gizmos.color = Color.magenta;
            Gizmos.DrawRay(apriltag.Position, forwardDirection);
        }
    }
}
