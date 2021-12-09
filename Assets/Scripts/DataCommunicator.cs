using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DataCommunicator : NetworkBehaviour
{

    private void Start()
    {
        if (GazeManager.Instance != null)
        {
            GazeManager.Instance.onRaycastHit.AddListener(SendDataToMaster);
        }
    }

    public void SendDataToMaster(Vector2 _vector2)
    {
        ReceiveFromClient(_vector2);
    }

    [Command]
    void ReceiveFromClient(Vector2 _vector2)
    {
        Debug.Log("Receiving data");
        Debug.Log(_vector2);
    }
}
