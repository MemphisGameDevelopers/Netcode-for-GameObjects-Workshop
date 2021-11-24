using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkVisibility : NetworkBehaviour
{
    NetworkObject netObject;
    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        netObject = GetComponent<NetworkObject>();
        netObject.CheckObjectVisibility = ((clientId) => {
            // return true to show the object, return false to hide it
            if (Vector3.Distance(NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.transform.position, transform.position) < 5)
            {
                // Only show the object to players that are within 5 meters. 
                // Note that this has to be rechecked by your own code
                // If you want it to update as the client and objects distance change.
                // This callback is usually only called once per client
                return true;
            }
            else
            {
                // Dont show this object
                return false;
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var client in NetworkManager.Singleton.ConnectedClients)
        {
            netObject.CheckObjectVisibility(client.Value.ClientId);
        }
        
    }
}
