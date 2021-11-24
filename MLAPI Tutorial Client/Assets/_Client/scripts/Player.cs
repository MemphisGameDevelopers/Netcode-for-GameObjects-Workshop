using System;
using UnityEngine;
// using MLAPI;
// using MLAPI.Serialization;
// using MLAPI.NetworkVariable;
// using MLAPI.Messaging;
using Unity.Netcode;
using Unity.Collections;

[System.Serializable]
public struct PlayerData : INetworkSerializable, IEquatable<PlayerData>
{
    public int Id;
    public FixedString32Bytes DisplayName;      
    public PlayAreaData Deck;

    // public void NetworkSerialize(NetworkSerializer serializer)
    // {
    //     serializer.Serialize(ref Id);
    //     serializer.Serialize(ref DisplayName);
    //     Deck.NetworkSerialize(serializer);        
    // }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Id);
        serializer.SerializeValue(ref DisplayName);
        Deck.NetworkSerialize(serializer);  
    }
     
    public bool Equals(PlayerData other)
    {
        return Id == other.Id && DisplayName.Equals(other.DisplayName) && 
                Deck.Equals(other.Deck);
    }
}



public class Player : NetworkBehaviour
{
    GameManager _manager;
    public PlayerDataSO player;
    public PlayerData Data = new PlayerData();  
    //public NetworkVariable<PlayerData> NetData = new NetworkVariable<PlayerData>(); 
    public NetworkVariable<FixedString32Bytes> DisplayName = new NetworkVariable<FixedString32Bytes>();
    public NetworkVariable<int> Health = new NetworkVariable<int>(100);
    public NetworkVariable<int> Money = new NetworkVariable<int>(0);
    public NetworkList<int> Deck = new NetworkList<int>();
    public NetworkList<int> Hand = new NetworkList<int>();
    public NetworkList<int> Field = new NetworkList<int>();
    public NetworkList<int> Discard = new NetworkList<int>();
    public NetworkVariable<bool> isActive = new NetworkVariable<bool>(false);
    

    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _manager = GameObject.FindObjectOfType<GameManager>();
        NetworkManager.Singleton.OnClientConnectedCallback += OnConnect;  
        //NetData.OnValueChanged += DataChange;
        DisplayName.OnValueChanged += NameChanged;  
        Health.OnValueChanged += HealthChanged;
        Money.OnValueChanged += MoneyChanged;
        Deck.OnListChanged += DeckChanged;
        Hand.OnListChanged += HandChanged;
        Field.OnListChanged += FieldChanged;
        Discard.OnListChanged += DiscardChanged;             
    }

    [ServerRpc]
    public void SendPlayerDataToServerRPC(PlayerData player){}

    void OnConnect(ulong client)
    {
        Data.DisplayName = player.data.DisplayName;
        Data.Id = (int)NetworkManager.Singleton.LocalClientId;
        Data.Deck = player.data.Deck;
        SendPlayerDataToServerRPC(Data);
        GameManager.Instance.SendPlayertoServerRPC(Data);
   
    }

    void DataChange(PlayerData oldData, PlayerData newData)
    {
        //print($"{newData.DisplayName.Value} has joined the game!");
    }

    void ActivePlayerChange(bool oldValue, bool newValue)
    {

    }

    void NameChanged(FixedString32Bytes oldValue, FixedString32Bytes newValue)
    {
        print($"Name changed to {newValue}");
    }

    void HealthChanged(int oldValue, int newValue)
    {

    }
    void MoneyChanged(int oldValue, int newValue)
    {

    }

    void DeckChanged(NetworkListEvent<int> chgEvt)
    {

    }
    void HandChanged(NetworkListEvent<int> chgEvt)
    {
        
    }
    void FieldChanged(NetworkListEvent<int> chgEvt)
    {
        
    }
    void DiscardChanged(NetworkListEvent<int> chgEvt)
    {
        
    }
}
