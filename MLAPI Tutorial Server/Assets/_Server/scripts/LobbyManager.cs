using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Authentication;
using System.Threading.Tasks;
using System.Collections.Concurrent;

public class LobbyManager : MonoBehaviour
{
    ConcurrentQueue<string> createdLobbyIds = new ConcurrentQueue<string>();
    
    // Start is called before the first frame update
    void Start()
    {
        AuthenticationService.Instance.SignedIn += async () =>
        {
          await CreateLobbyAsync("Test Lobby", 10);  
        }; 
            
    }

    public async Task CreateLobbyAsync(string lobbyName, int maxPlayers)
    {
        CreateLobbyOptions options = new CreateLobbyOptions();
        options.IsPrivate = false;

        options.Data = new Dictionary<string, DataObject>()
        {
            {
                "IndexedPulicLobbyData",new DataObject(
                visibility: DataObject.VisibilityOptions.Public,
                value: "IndexedPulicLobbyData",
                index: DataObject.IndexOptions.S1)
            }
        };

        options.Player = new Unity.Services.Lobbies.Models.Player(
            id: AuthenticationService.Instance.PlayerId,
            data: new Dictionary<string, PlayerDataObject>()
            {
                {
                    "MemberPlayerData", new PlayerDataObject(
                    visibility: PlayerDataObject.VisibilityOptions.Member,
                    value: "MemberPlayerData")
                }
            }
        );

        Lobby lobby = await Lobbies.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
        createdLobbyIds.Enqueue(lobby.Id);
        StartCoroutine(HeartbeatLobbyCoroutine(lobby.Id, 15));

    }

    IEnumerator HeartbeatLobbyCoroutine(string lobbyId, float waitTimeSeconds)
    {
        var delay = new WaitForSecondsRealtime(waitTimeSeconds);
        while (true)
        {
            Lobbies.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return delay;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnApplicationQuit()
    {
        while (createdLobbyIds.TryDequeue(out var lobbyId))
    	{
        	Lobbies.Instance.DeleteLobbyAsync(lobbyId);
    	}
    }
}
