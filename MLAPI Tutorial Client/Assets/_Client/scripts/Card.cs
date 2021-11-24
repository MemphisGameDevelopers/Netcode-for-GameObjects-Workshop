using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
//using MLAPI.Serialization;

[System.Serializable]
public class CardData : ScriptableObject, INetworkSerializable
{
    public int Id;
    //public string Name;
    public int Power;

    // public void NetworkSerialize(NetworkSerializer serializer)
    // {
    //     serializer.Serialize(ref Id);
    //     //serializer.Serialize(ref Name);
    //     serializer.Serialize(ref Power);
    // }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Id);
        serializer.SerializeValue(ref Power);
    }
}
public class Card : NetworkBehaviour
{
    public CardData data;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
