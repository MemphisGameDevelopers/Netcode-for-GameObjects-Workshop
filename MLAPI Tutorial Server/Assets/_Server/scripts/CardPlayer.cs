using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Collections;
using Unity.Netcode;

[System.Serializable]
public struct PlayerData : INetworkSerializable
{
    public int Id;
    public FixedString32Bytes DisplayName;
      
    public PlayAreaData Deck;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Id);
        serializer.SerializeValue(ref DisplayName);
        Deck.NetworkSerialize(serializer); 
    }
}



public class CardPlayer : NetworkBehaviour
{
    //public PlayerData _playerData;  
    //public NetworkVariable<PlayerData> NetData = new NetworkVariable<PlayerData>();
    public NetworkVariable<FixedString32Bytes> DisplayName = new NetworkVariable<FixedString32Bytes>();
    public NetworkVariable<int> Health = new NetworkVariable<int>(100);
    public NetworkVariable<int> Money = new NetworkVariable<int>(0);
    public NetworkList<int> Deck = new NetworkList<int>();
    public NetworkList<int> Hand = new NetworkList<int>();
    public NetworkList<int> Field = new NetworkList<int>();
    public NetworkList<int> Discard = new NetworkList<int>();
    public NetworkVariable<bool> isActive = new NetworkVariable<bool>(false);
    NetworkObject netObject;

    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnConnect;  
        netObject = GetComponent<NetworkObject>();
        
               
    }

    void OnConnect(ulong client)
    {
       // NetData.Value = GameManager.Instance.playersDict[client]; 
      
    }

    [ServerRpc]
    public void SendPlayerDataToServerRPC(PlayerData player)
    {
        DisplayName.Value = player.DisplayName;
        //Deck.Value = player.Deck;
    }

    
}
