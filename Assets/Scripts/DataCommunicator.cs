using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DataCommunicator : NetworkBehaviour
{
    public Vector2 raycastHit = Vector2.zero;

    private void Update()
    {
        if (isLocalPlayer)
        {
            ReceiveFromClient(raycastHit);
        }
    }

    public void SendDataToMaster(Vector2 _vector2)
    {
        raycastHit = _vector2;
    }

    [Command]
    void ReceiveFromClient(Vector2 _vector2)
    {
        Debug.Log("Receiving data");
        Debug.Log(_vector2);
    }
}
