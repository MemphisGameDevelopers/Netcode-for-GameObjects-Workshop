using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using MLAPI.Serialization;
// using MLAPI.Messaging;
// using MLAPI.Serialization.Pooled;
using Unity.Netcode;
using Unity.Collections;

public enum MsgType
{
    PlayerConnected,
    StartGame,
    StartTurn,
    MoveCard,
    EndTurn,
    EndGame,
    Notification
}

public interface NetworkMsg : INetworkSerializable{}

[System.Serializable]
public struct StartGameMsg : NetworkMsg
{
    public PlayerData localPlayer, remotePlayer;

    // public void NetworkSerialize(NetworkSerializer serializer)
    // {
    //     localPlayer.NetworkSerialize(serializer);
    //     remotePlayer.NetworkSerialize(serializer);
    // }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        localPlayer.NetworkSerialize(serializer);
        remotePlayer.NetworkSerialize(serializer);
    }
}

public struct StartTurnMsg : NetworkMsg
{
    public PlayerData localPlayer, remotePlayer;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        localPlayer.NetworkSerialize(serializer);
        remotePlayer.NetworkSerialize(serializer);
    }

}

[System.Serializable]
public struct MoveCardMsg : NetworkMsg
{
    public int playerId, cardId, from_AreaId, to_AreaId;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref playerId);
        serializer.SerializeValue(ref cardId);
        serializer.SerializeValue(ref from_AreaId);
        serializer.SerializeValue(ref to_AreaId);        
    }
}

public struct NotificationMsg : NetworkMsg
{
    public FixedString32Bytes message;
    // public void NetworkSerialize(NetworkSerializer serializer)
    // {
    //     serializer.Serialize(ref message);
    // }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref message);
    }
}
