using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Calculate hit of the gaze on the table using RayCasting
/// </summary>
public class GazeHandler : MonoBehaviour
{
    /// <summary>
    /// Event for when the raycast hits (which happens when looking at the table)
    /// </summary>
    public UnityEvent<Vector2> onRaycastHit;

    /// <summary>
    /// Event which is trigger when the data for 1 frame is processed
    /// </summary>
    public UnityEvent onProcessingComplete;

    /// <summary>
    /// Layermask where ray will be drawn
    /// </summary>
    [SerializeField] LayerMask layerMask;

    /// <summary>
    /// The ray for the gaze direction
    /// </summary>
    private Ray ray;

    /// <summary>
    /// Calculate the gaze and the hit on the table
    /// </summary>
    /// <param name="_gazePosition">The position of the gaze in values between 0 and 1</param>
    public void CalculateGazeHit(Vector2 _gazePosition)
    {
        // Calculate coordinates relative to camera
        _gazePosition = new Vector2(_gazePosition.x * Camera.main.pixelWidth, _gazePosition.y * Camera.main.pixelHeight);

        // Cast a ray
        ray = Camera.main.ScreenPointToRay(_gazePosition);

        // Check for hit
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            // Calculate relative position and trigger event
            Vector3 hitRelative = hit.transform.InverseTransformPoint(hit.point);
            onRaycastHit?.Invoke(new Vector2(hitRelative.x, hitRelative.z));
        }

        DetectorManager.Instance.raycastTime = Time.realtimeSinceStartupAsDouble - DetectorManager.Instance.startTime;
        DetectorManager.Instance.startTime = Time.realtimeSinceStartupAsDouble;

        File.AppendAllText("LOGS.txt", (
            DetectorManager.Instance.frame + "; " +
            DetectorManager.Instance.to2DTime + "; " +
            DetectorManager.Instance.detectingTime + "; " +
            DetectorManager.Instance.tableTime + "; " +
            DetectorManager.Instance.searchGazeTime + "; " +
            DetectorManager.Instance.raycastTime + "; \n"
            ));

        // Trigger onProcessingComplete event
        onProcessingComplete?.Invoke();
    }

    /// <summary>
    /// Drawing of gizmos
    /// </summary>
    private void OnDrawGizmos()
    {
        // Check for camera
        if (Camera.main != null)
        {
            // Cast a ray
            Gizmos.color = Color.green;
            Gizmos.DrawRay(ray.origin, ray.direction * 1000);

            // Check for hit
            if (Physics.Raycast(ray, out RaycastHit hit, 1000, layerMask))
            {
                // Visualize hit
                Gizmos.DrawSphere(hit.point, .005f);
            }
        }
    }
}
