using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;
using TMPro;
using Unity.Netcode;

public class ServerMessageHandler : MonoBehaviour
{

    void Awake()
    {
       NetworkManager.Singleton.CustomMessagingManager.OnUnnamedMessage += HandleUnnamedMessage;        
    }

    void Start()
    {
        NetworkManager.Singleton.StartServer();
    }

    void HandleUnnamedMessage(ulong sender, FastBufferReader reader)
    {
        reader.ReadValueSafe(out string msg);
        Send(sender,$"Server sending named message to Client {sender} containing message {msg}");
    }

    void Send(ulong client, string msg)
    {        
        using var writer = new FastBufferWriter();
        writer.WriteValueSafe<NotificationMsg>(new NotificationMsg{message = msg});

        NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage(MsgType.Notification.ToString(), client, writer);
    
        //CustomMessagingManager.SendUnnamedMessage(null, buffer);
    }
}
