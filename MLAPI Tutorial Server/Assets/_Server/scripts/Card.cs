using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

[System.Serializable]
public struct CardData : INetworkSerializable, IEquatable<CardData>
{
    public int Id;
    public FixedString32Bytes Name;
    public int Power;

    public bool Equals(CardData other)
    {
        throw new NotImplementedException();
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Id);
        serializer.SerializeValue(ref Name);
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
