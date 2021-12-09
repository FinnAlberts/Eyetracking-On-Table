using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GazeManager : MonoBehaviour
{
    /// <summary>
    /// The camera
    /// </summary>
    public Camera cam;

    /// <summary>
    /// Layermask where ray will be drawn
    /// </summary>
    public LayerMask layerMask;

    /// <summary>
    /// Event for when the raycast hits (which happens when looking at the table)
    /// </summary>
    public UnityEvent<Vector2> onRaycastHit;

    // Making GazeManager into an singleton
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

    private void Update()
    {
        // Check for camera
        if (cam != null)
        {
            // Cast a ray
            Ray ray = cam.ScreenPointToRay(InputManager.Instance.mousePosition);

            // Initialize hit variable
            RaycastHit hit;

            // Check for hit
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {

                // Calculate relative position and trigger event
                Vector3 hitRelative = hit.transform.InverseTransformPoint(hit.point);
                onRaycastHit?.Invoke(new Vector2(hitRelative.x, hitRelative.z));
                Debug.Log(hitRelative);
            }
        }
    }

    // Drawing of Gizmos
    private void OnDrawGizmos()
    {
        // Check for camera
        if (cam != null && InputManager.Instance != null)
        { 
            // Cast a ray
            Ray ray = cam.ScreenPointToRay(InputManager.Instance.mousePosition);
            Gizmos.color = Color.green;
            Gizmos.DrawRay(ray.origin, ray.direction * Mathf.Infinity);

            // Initialize hit variable
            RaycastHit hit;

            // Check for hit
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                // Visualize hit
                Gizmos.DrawSphere(hit.point, 0.005f);
            }
        }
    }
}
