using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gaze : MonoBehaviour
{
    /// <summary>
    /// The camera
    /// </summary>
    public Camera camera;

    /// <summary>
    /// Layermask where ray will be drawn
    /// </summary>
    public LayerMask layerMask;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDrawGizmos()
    {
        if (camera != null)
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            Gizmos.color = Color.green;
            Gizmos.DrawRay(ray.origin, ray.direction * Mathf.Infinity);

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                Gizmos.DrawSphere(hit.point, 1);
            }
        }
    }
}
