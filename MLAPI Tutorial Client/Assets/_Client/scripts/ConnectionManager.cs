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

public class ConnectionManager : MonoBehaviour
{
    public string RelayJoinCode;

    // Start is called before the first frame update
    public async void Start()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        //AuthenticationService.Instance.SignedIn += async()=> StartClient();
    }

    public async void StartClient()
    {
        // Populate RelayJoinCode beforehand through the UI
        var clientRelayUtilityTask = JoinRelayServerFromJoinCode(RelayJoinCode);

        while (!clientRelayUtilityTask.IsCompleted)
        {
            await Task.Yield();
        }

        if (clientRelayUtilityTask.IsFaulted)
        {
            Debug.LogError("Exception thrown when attempting to connect to Relay Server. Exception: " + clientRelayUtilityTask.Exception.Message);
            await Task.FromException(clientRelayUtilityTask.Exception);
        }

        var (ipv4address, port, allocationIdBytes, connectionData, hostConnectionData, key) = clientRelayUtilityTask.Result;

        // When connecting as a client to a Relay server, connectionData and hostConnectionData are different.
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(ipv4address, port, allocationIdBytes, key, connectionData, hostConnectionData);

        print($"Starting Client on {ipv4address}:{port}");
        NetworkManager.Singleton.StartClient();
        await Task.Yield();
    }

    public static async Task<(string ipv4address, ushort port, byte[] allocationIdBytes, byte[] connectionData, byte[] hostConnectionData, byte[] key)> JoinRelayServerFromJoinCode(string joinCode)
    {
        JoinAllocation allocation;
        try
        {
            allocation = await Relay.Instance.JoinAllocationAsync(joinCode);
        }
        catch
        {
            Debug.LogError("Relay create join code request failed");
            throw;
        }

        Debug.Log($"client: Connection Data[0] {allocation.ConnectionData[0]} Connection Data[1] {allocation.ConnectionData[1]}");
        Debug.Log($"host: Connection Data[0] {allocation.HostConnectionData[0]} Connection Data[1] {allocation.HostConnectionData[1]}");
        Debug.Log($"client: Allocation ID {allocation.AllocationId}");

        return (allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port, allocation.AllocationIdBytes, allocation.ConnectionData, allocation.HostConnectionData, allocation.Key);
    }

    public async void QuickJoin()
    {
        try
        {
        
        QuickJoinLobbyOptions options = new QuickJoinLobbyOptions();
        options.Filter = new List<QueryFilter>()
        {
            new QueryFilter(
                field: QueryFilter.FieldOptions.MaxPlayers,
                op: QueryFilter.OpOptions.GE,
                value: "10"
            )
        };

        options.Player = new Unity.Services.Lobbies.Models.Player(

        );
    

        Lobby lobby = await Lobbies.Instance.QuickJoinLobbyAsync(options);
        print($"Joined Lobby: {lobby.Name}");
        RelayJoinCode = lobby.Data["RelayCode"].Value;
        StartClient();
        }
        catch(LobbyServiceException error)
        {
            print(error);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
