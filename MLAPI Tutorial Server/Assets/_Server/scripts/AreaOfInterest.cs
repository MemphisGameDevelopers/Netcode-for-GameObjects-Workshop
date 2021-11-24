using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class AreaOfInterest : NetworkBehaviour
{

    void OnTriggerEnter(Collider col)
    {
        var netObj = col.gameObject.GetComponent<NetworkObject>();
        if(netObj != null && !netObj.IsNetworkVisibleTo(OwnerClientId))
        {  
            netObj.NetworkShow(OwnerClientId);
        }
    }

    void OnTriggerExit(Collider col)
    {
        var netObj = col.gameObject.GetComponent<NetworkObject>();
        if(netObj != null && netObj.IsNetworkVisibleTo(OwnerClientId))
        {
            netObj.NetworkHide(OwnerClientId);
        }
    }
}
