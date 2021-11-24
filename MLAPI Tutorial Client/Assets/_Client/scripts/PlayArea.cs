using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using System.Linq;


[System.Serializable]
public struct PlayAreaData : INetworkSerializable
{
    //public string name;
    //public List<CardData> cardSetup;
    public int[] cards;

    

    public void Shuffle()
    {
              
        cards.OrderBy(c => Random.value);
    }

    public int[] Draw(int amount)
    {
        var _cards = cards.Take(amount).ToArray();
        cards = cards.Except(_cards).ToArray();
        return _cards;
    }

    public void MoveTo(int card, PlayAreaData area)
    {
        cards = cards.Except(new int[] {card}).ToArray();        
        area.cards.Append(card);        
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        int length = 0;
        if(!serializer.IsReader)
        {
            length = cards.Length;
        }
        serializer.SerializeValue(ref length);
        if(serializer.IsReader)
        {            
            cards = new int[length];
        }

        for(int i = 0; i< length; ++i)
        {
            serializer.SerializeValue(ref cards[i]);
        }
    }
}

public class PlayArea : NetworkBehaviour
{
    PlayAreaData _playArea;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
