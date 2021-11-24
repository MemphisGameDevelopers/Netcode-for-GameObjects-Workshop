using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    //public NetworkList<PlayerVariable> players = new NetworkList<PlayerVariable>();
    public Dictionary<ulong, PlayerData> playersDict = new Dictionary<ulong, PlayerData>();
    public List<GameObject> PlayerChars;


    void Awake()
    {
        
    }
    // Start is called before the first frame update
    void Start()
    {
        if(Instance == null) Instance = this; 
        NetworkManager.Singleton.OnClientConnectedCallback += OnConnect; 
        NetworkManager.Singleton.OnClientDisconnectCallback += OnDisconnect;
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        //NetworkManager.Singleton.StartServer();
    }

    void OnConnect(ulong client)
    {

    }

    void OnDisconnect(ulong client)
    {      
        if(!playersDict.ContainsKey(client)) return;
        print($"player {playersDict[client].DisplayName} has disconnected from Client {client}");
        playersDict.Remove(client);  
        // foreach (var player in players)
        // {
        //     if(player.Id == (int)client)
        //     {
        //         players.Remove(player);
        //         return;
        //     }
        // }
    }

    private void ApprovalCheck(byte[] connectionData, ulong clientId, NetworkManager.ConnectionApprovedDelegate callback)
    {
        //Your logic here
        bool approve = true;
        bool createPlayerObject = true;

        // The prefab hash. Use null to use the default player prefab
        // If using this hash, replace "MyPrefabHashGenerator" with the name of a prefab added to the NetworkPrefabs field of your NetworkManager object in the scene
        uint? prefabHash = null; //NetworkSpawnManager.GetPrefabHashFromGenerator(PlayerChars[Random.Range(0, PlayerChars.Count)].name);
        
        //If approve is true, the connection gets added. If it's false. The client gets disconnected
        callback(createPlayerObject, prefabHash, approve, Vector3.zero, Quaternion.identity);
    }


    [ServerRpc(RequireOwnership = false)]
    public void SendPlayertoServerRPC(PlayerData _player, ServerRpcParams RPCparams)
    {
        var client = RPCparams.Receive.SenderClientId;
        var player = NetworkManager.Singleton.ConnectedClients[client].PlayerObject.GetComponent<CardPlayer>();
        //players.Add(_player);
        playersDict.Add(client, _player);    
        print($"new player {_player.DisplayName} joined from Client {client}");
           
    }

    [ServerRpc]
    public void SpawnPlayerOnServerRPC(int CharacterIndex, ServerRpcParams RPCparams = default)
    {
        var obj = Instantiate(PlayerChars[CharacterIndex]);
        obj.GetComponent<NetworkObject>().SpawnAsPlayerObject(RPCparams.Receive.SenderClientId);
    }
}
