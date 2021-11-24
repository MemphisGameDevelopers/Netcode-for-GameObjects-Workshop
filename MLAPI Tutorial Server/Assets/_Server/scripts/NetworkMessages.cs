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

public interface NetworkMsg : INetworkSerializable {}

[System.Serializable]
public struct StartGameMsg : NetworkMsg
{
    //public PlayerData localPlayer, remotePlayer;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        
    }
}

[System.Serializable]
public struct MoveCardMsg : NetworkMsg
{
    public int playerId, cardId, from_AreaId, to_AreaId;


    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        throw new System.NotImplementedException();
    }
}

public struct NotificationMsg : NetworkMsg
{
    public FixedString32Bytes message;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref message);
    }
}
