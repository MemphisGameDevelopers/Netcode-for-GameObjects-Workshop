using System.Collections.Generic;
using UnityEngine;
// using MLAPI;
// using MLAPI.NetworkVariable.Collections;
// using MLAPI.Messaging;
// using MLAPI.Logging;
using Unity.Netcode;


public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    //public NetworkList<Player> players = new NetworkList<Player>();
    public Dictionary<ulong, Player> playersDict = new Dictionary<ulong, Player>();

    public string RoomName;
    public List<GameObject> PlayerChars;

    
    // Start is called before the first frame update
    void Awake()
    {
        //players.OnListChanged += PlayerListUpdated;       
                  
    }

     void Start()
    {        
        if(Instance == null) Instance = this;
        NetworkManager.Singleton.OnClientConnectedCallback += OnConnect; 
        NetworkManager.Singleton.OnClientDisconnectCallback += OnDisconnect;   
        NetworkManager.Singleton.NetworkConfig.ConnectionData = System.Text.Encoding.ASCII.GetBytes(RoomName);        
        //NetworkManager.Singleton.StartClient();
    }

    void OnConnect(ulong client)
    {
        NetworkLog.LogInfoServer("Client has connected!");
    }

    void OnDisconnect(ulong client)
    {
        NetworkLog.LogInfoServer("Client has disconnected!");
    }



    void PlayerListUpdated(NetworkListEvent<Player> listchange)
    {
        switch (listchange.Type)
        {
            case NetworkListEvent<Player>.EventType.Add:
            print($"(List)Player {listchange.Value.DisplayName.Value} has joined the game!");
            break;

            case NetworkListEvent<Player>.EventType.Remove:
             print($"(List)Player {listchange.Value.DisplayName.Value} has left the game!");
            break;            
        }
        
    }
    // void PlayerDictUpdated(NetworkDictionaryEvent<ulong, Player> dictChange)
    // {
    //     print($"(Dict)Player has joined the game!");
    // }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ServerRpc]
    public void SpawnPlayerOnServerRPC(int CharacterIndex, ServerRpcParams RPCparams = default){}

    [ServerRpc(RequireOwnership = false)]
    public void SendPlayertoServerRPC(PlayerData _player, ServerRpcParams RPCparams = default){}

}
