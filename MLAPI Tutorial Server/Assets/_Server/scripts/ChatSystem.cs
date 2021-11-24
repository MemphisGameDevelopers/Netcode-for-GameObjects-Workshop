using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;



public class ChatSystem : NetworkBehaviour
{  
    NetworkList<FixedString32Bytes> ChatMessages; 

    [ServerRpc(RequireOwnership = false)]
    void MessagetoServerRPC()
    {
        print("OMG It Works!!!");
    } 
    // Start is called before the first frame update
    void Start()
    {
        ChatMessages = new NetworkList<FixedString32Bytes>(); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
