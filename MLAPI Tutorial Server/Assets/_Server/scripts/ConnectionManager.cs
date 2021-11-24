using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class ConnectionManager : NetworkBehaviour
{
    public int MaxConnections = 10;
    Lobby lobby;
    // Start is called before the first frame update
    public async void Awake()
    {
        await UnityServices.InitializeAsync();
        AuthenticationService.Instance.SignedIn += () => StartCoroutine(StartHost());
    }
    public IEnumerator StartHost()
    {        
        var serverRelayUtilityTask = AllocateRelayServerAndGetJoinCode(MaxConnections);
        while (!serverRelayUtilityTask.IsCompleted)
        {
            yield return null;
        }
        if (serverRelayUtilityTask.IsFaulted)
        {
            Debug.LogError("Exception thrown when attempting to start Relay Server. Server not started. Exception: " + serverRelayUtilityTask.Exception.Message);
            yield break;
        }

        var (ipv4address, port, allocationIdBytes, connectionData, key, joinCode) = serverRelayUtilityTask.Result;
        // Display the joinCode to the user.
        print($"joinCode: {joinCode}");

        var task = CreateLobby(joinCode);             

        // When starting a Relay server, both instances of connection data are identical.
        print($"Starting Server on {ipv4address} : {port}");
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(ipv4address, port, allocationIdBytes, key, connectionData);
        NetworkManager.Singleton.StartServer();
        yield return null;        
    }

    public static async Task<(string ipv4address, ushort port, byte[] allocationIdBytes, byte[] connectionData, byte[] key, string joinCode)> AllocateRelayServerAndGetJoinCode(int maxConnections, string region = null)
    {
        Allocation allocation;
        string createJoinCode;
        try
        {
            allocation = await Relay.Instance.CreateAllocationAsync(maxConnections, region);
        }
        catch (Exception e)
        {
            Debug.LogError($"Relay create allocation request failed {e.Message}");
            throw;
        }

        Debug.Log($"server Connection Data: {allocation.ConnectionData[0]} {allocation.ConnectionData[1]}");
        Debug.Log($"server Allocation ID: {allocation.AllocationId}");

        try
        {
            createJoinCode = await Relay.Instance.GetJoinCodeAsync(allocation.AllocationId);
        }
        catch
        {
            Debug.LogError("Relay create join code request failed");
            throw;
        }

        return (allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port, allocation.AllocationIdBytes, allocation.ConnectionData, allocation.Key, createJoinCode);
    }

    async Task CreateLobby(string relayCode)
    {
        try
        {
            print("Creating Lobby...");
            CreateLobbyOptions options = new CreateLobbyOptions();
            options.IsPrivate = false;
            options.Data = new Dictionary<string, DataObject>()
            {
                {
                    "RelayCode", new DataObject(
                        visibility: DataObject.VisibilityOptions.Member,
                        value: relayCode,
                        index: DataObject.IndexOptions.S1
                    )
                }
            };
            lobby = await Lobbies.Instance.CreateLobbyAsync("Test Lobby", MaxConnections, options);
            print($"Lobby {lobby.Id} is created wuth Lobby code {lobby.LobbyCode}");
            StartCoroutine(HearbeatLobby(lobby.Name, 15));
        }
        catch (LobbyServiceException e)
        {            
            Debug.Log(e);
        }  
    }

    IEnumerator HearbeatLobby(string lobbyId, float waitTimeinSecs)
    {
        var delay = new WaitForSecondsRealtime(waitTimeinSecs);
    	while (true)
    	{
            print("Lobby hearbeat...");
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
        Lobbies.Instance.DeleteLobbyAsync(lobby.Id);
    }
}
