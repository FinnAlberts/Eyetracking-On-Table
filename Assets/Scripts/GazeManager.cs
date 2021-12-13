using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GazeManager : MonoBehaviour
{
    /// <summary>
    /// Layermask where ray will be drawn
    /// </summary>
    public LayerMask layerMask;

    /// <summary>
    /// Event for when the raycast hits (which happens when looking at the table)
    /// </summary>
    public UnityEvent<Vector2> onRaycastHit;

    #region Making GazeManager into an singleton
    private static GazeManager _instance;

    public static GazeManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    #endregion

    public Ray ray;

    public void CalculateGaze(Vector2 _gazePosition)
    {
        // Calculate coordinates relative to camera
        _gazePosition = new Vector2(_gazePosition.x * Camera.main.pixelWidth, _gazePosition.y * Camera.main.pixelHeight);

        // Cast a ray
        ray = Camera.main.ScreenPointToRay(_gazePosition);

        // Initialize hit variable
        RaycastHit hit;

        // Check for hit
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            // Calculate relative position and trigger event
            Vector3 hitRelative = hit.transform.InverseTransformPoint(hit.point);
            onRaycastHit?.Invoke(new Vector2(hitRelative.x, hitRelative.z));
        }
        
    }

    // Drawing of Gizmos
    private void OnDrawGizmos()
    {
        // Check for camera
        if (Camera.main != null)
        {
            // Cast a ray
            Gizmos.color = Color.green;
            Gizmos.DrawRay(ray.origin, ray.direction * 1000);

            // Initialize hit variable
            RaycastHit hit;

            // Check for hit
            if (Physics.Raycast(ray, out hit, 1000, layerMask))
            {
                // Visualize hit
                Gizmos.DrawSphere(hit.point, .005f);
            }
        }
    }
}
