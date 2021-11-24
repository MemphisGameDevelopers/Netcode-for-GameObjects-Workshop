using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using Unity.Netcode;
//using MLAPI.Messaging;
//using MLAPI.Serialization.Pooled;
using System.Text;

public class ClientMessageHandler : MonoBehaviour
{    
    public static UnityEvent<StartGameMsg> StartGameEvt;
    public static UnityEvent<NotificationMsg> NotificationEvt;

    void Awake()
    {
        NetworkManager.Singleton.CustomMessagingManager.OnUnnamedMessage += (serverID, stream) =>
        {
            //using var reader = PooledNetworkReader.Get(stream);
            //string msg = reader.ReadString(new StringBuilder(10)).ToString();
            stream.ReadValueSafe(out string msg);
            print(msg);
        };

        NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler(MsgType.Notification.ToString(), (serverID, stream) =>
        {            
            //NotificationMsg msg = new NotificationMsg();
            //using var reader = PooledNetworkReader.Get(stream);
            //msg =(NotificationMsg)reader.ReadObjectPacked(typeof(NotificationMsg));
            stream.ReadValueSafe<NotificationMsg>(out NotificationMsg msg);           
            NotificationEvt?.Invoke(msg);                     
              
        });
    }

    public void Send(string msg)
    {
        // using var buffer = PooledNetworkBuffer.Get();
        // using var writer = PooledNetworkWriter.Get(buffer);
        using var writer = new FastBufferWriter();
        writer.WriteValueSafe(msg);
        NetworkManager.Singleton.CustomMessagingManager.SendUnnamedMessage(NetworkManager.Singleton.ServerClientId, writer);
        print("Unnamed Message Sent!");
    }

    public void SendMsg(string msgType, NetworkMsg msg)
    {        
        // using var buffer = PooledNetworkBuffer.Get();
        // using var writer = PooledNetworkWriter.Get(buffer);

        var newMsg = (NotificationMsg)msg;
        using var writer = new FastBufferWriter();
        writer.WriteValueSafe<NotificationMsg>(in newMsg);
        NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage(msgType, NetworkManager.Singleton.ServerClientId, writer);
    }
}
