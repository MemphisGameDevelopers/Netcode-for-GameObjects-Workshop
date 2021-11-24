using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
//using MLAPI.NetworkVariable;
//using MLAPI.NetworkVariable.Collections;
using Unity.Collections;
using TMPro;


public class ChatSystem : NetworkBehaviour
{   
    NetworkList<FixedString32Bytes> ChatMessages; 
    /* (new NetworkVariableSettings
    {
        ReadPermission = NetworkVariablePermission.Everyone,
        WritePermission = NetworkVariablePermission.Everyone,        
    }, new List<string>());     */
    public TMP_InputField chatInput;
    public TMP_Text ChatBox;

    void Awake()
    {
        ChatMessages = new NetworkList<FixedString32Bytes>();
        ChatMessages.OnListChanged += ChatUpdated;
    }
    public void SendChat(string msg)
    {
        if(!string.IsNullOrWhiteSpace(chatInput.text))
        {
            ChatMessages.Add(msg);
            chatInput.text = "";
        }
    }

    public void ChatUpdated(NetworkListEvent<FixedString32Bytes> chatEvt)
    {
        print("Chat updated");
        ChatBox.text = "";
        foreach (var msg in ChatMessages)
        {
            ChatBox.text += msg + "\n";
        }
    }


}
