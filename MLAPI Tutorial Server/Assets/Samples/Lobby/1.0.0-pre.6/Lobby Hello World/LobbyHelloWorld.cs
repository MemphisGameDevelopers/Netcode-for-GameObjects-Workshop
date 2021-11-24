using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Random = UnityEngine.Random;

/// <summary>
/// A simple "Hello World" style example using Lobby, exercising most of the Lobby APIs.
/// 
/// SETUP:
///  1. Attach this script to a GameObject in scene
///  2. Sign in to your Unity developer account
///  3. Link your project to a Lobby-enabled cloud project
///  4. Enter play mode and observe the debug logs
/// 
/// </summary>
public class LobbyHelloWorld : MonoBehaviour
{
    // Inspector properties with initial values

    /// <summary>
    /// Used to set the lobby name in this example.
    /// </summary>
    public string newLobbyName = "LobbyHelloWorld" + Guid.NewGuid();

    /// <summary>
    /// Used to set the max number of players in this example.
    /// </summary>
    public int maxPlayers = 8;

    /// <summary>
    /// Used to determine if the lobby shall be private in this example.
    /// </summary>
    public bool isPrivate = false;

    // We'll only be in one lobby at once for this demo, so let's track it here
    private Lobby currentLobby;

    async void Start()
    {
        try
        {
            await ExecuteLobbyDemoAsync();
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }

        await CleanupDemoLobbyAsync();

        Debug.Log("Demo complete!");
    }

    // Clean up the lobby we're in if we're the host
    async Task CleanupDemoLobbyAsync()
    {
        var localPlayerId = AuthenticationService.Instance.PlayerId;

        // This is so that orphan lobbies aren't left around in case the demo fails partway through
        if (currentLobby != null && currentLobby.HostId.Equals(localPlayerId))
        {
            await Lobbies.Instance.DeleteLobbyAsync(currentLobby.Id);
            Debug.Log($"Deleted lobby {currentLobby.Name} ({currentLobby.Id})");
        }
    }

    // A basic demo of lobby functionality
    async Task ExecuteLobbyDemoAsync()
    {
        await UnityServices.InitializeAsync();

        // Log in a player for this game client
        Player loggedInPlayer = await GetPlayerFromAnonymousLoginAsync();

        // Add some data to our player
        // This data will be included in a lobby under players -> player.data
        loggedInPlayer.Data.Add("Ready", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, "No"));

        // Query for existing lobbies

        // Use filters to only return lobbies which match specific conditions
        // You can only filter on built-in properties (Ex: AvailableSlots) or indexed custom data (S1, N1, etc.)
        // Take a look at the API for other built-in fields you can filter on
        List<QueryFilter> queryFilters = new List<QueryFilter>
        {
            // Let's search for games with open slots (AvailableSlots greater than 0)
            new QueryFilter(
                field: QueryFilter.FieldOptions.AvailableSlots,
                op: QueryFilter.OpOptions.GT,
                value: "0"),

            // Let's add some filters for custom indexed fields
            new QueryFilter(
                field: QueryFilter.FieldOptions.S1, // S1 = "Test"
                op: QueryFilter.OpOptions.EQ,
                value: "true"),

            new QueryFilter(
                field: QueryFilter.FieldOptions.S2, // S2 = "GameMode"
                op: QueryFilter.OpOptions.EQ,
                value: "ctf"),

            // Example "skill" range filter (skill is a custom numeric field in this example)
            new QueryFilter(
                field: QueryFilter.FieldOptions.N1, // N1 = "Skill"
                op: QueryFilter.OpOptions.GT,
                value: "0"),

            new QueryFilter(
                field: QueryFilter.FieldOptions.N1, // N1 = "Skill"
                op: QueryFilter.OpOptions.LT,
                value: "51"),
        };

        // Query results can also be ordered
        // The query API supports multiple "order by x, then y, then..." options
        // Order results by available player slots (least first), then by lobby age, then by lobby name
        List<QueryOrder> queryOrdering = new List<QueryOrder>
        {
            new QueryOrder(true, QueryOrder.FieldOptions.AvailableSlots),
            new QueryOrder(false, QueryOrder.FieldOptions.Created),
            new QueryOrder(false, QueryOrder.FieldOptions.Name),
        };

        // Call the Query API
        QueryResponse response = await Lobbies.Instance.QueryLobbiesAsync(new QueryLobbiesOptions()
        {
            Count = 20, // Override default number of results to return
            Filters = queryFilters,
            Order = queryOrdering,
        });

        List<Lobby> foundLobbies = response.Results;

        if (foundLobbies.Any()) // Try to join a random lobby if one exists
        {
            // Let's print info about the lobbies we found
            Debug.Log("Found lobbies:\n" + JsonConvert.SerializeObject(foundLobbies));

            // Let's pick a random lobby to join
            var randomLobby = foundLobbies[Random.Range(0, foundLobbies.Count)];

            // Try to join the lobby
            // Player is optional because the service can pull the player data from the auth token
            // However, if your player has custom data, you will want to pass the Player object into this call
            // This will save you having to do a Join call followed by an UpdatePlayer call
            currentLobby = await Lobbies.Instance.JoinLobbyByIdAsync(
                lobbyId: randomLobby.Id,
                options: new JoinLobbyByIdOptions()
                {
                    Player = loggedInPlayer
                });

            Debug.Log($"Joined lobby {currentLobby.Name} ({currentLobby.Id})");

            // You can also join via a Lobby Code instead of a lobby ID
            // Lobby Codes are a short, unique codes that map to a specific lobby ID
            // EX:
            // currentLobby = await Lobbies.Instance.JoinLobbyByCodeAsync("myLobbyJoinCode");
        }
        else // Didn't find any lobbies, create a new lobby
        {
            // Populate the new lobby with some data; use indexes so it's easy to search for
            var lobbyData = new Dictionary<string, DataObject>()
            {
                ["Test"] = new DataObject(DataObject.VisibilityOptions.Public, "true", DataObject.IndexOptions.S1),
                ["GameMode"] = new DataObject(DataObject.VisibilityOptions.Public, "ctf", DataObject.IndexOptions.S2),
                ["Skill"] = new DataObject(DataObject.VisibilityOptions.Public, Random.Range(1, 51).ToString(), DataObject.IndexOptions.N1),
                ["Rank"] = new DataObject(DataObject.VisibilityOptions.Public, Random.Range(1, 51).ToString()),
            };

            // Create a new lobby
            currentLobby = await Lobbies.Instance.CreateLobbyAsync(
                lobbyName: newLobbyName,
                maxPlayers: maxPlayers,
                options: new CreateLobbyOptions()
                {
                    Data = lobbyData,
                    IsPrivate = isPrivate,
                    Player = loggedInPlayer
                });

            Debug.Log($"Created new lobby {currentLobby.Name} ({currentLobby.Id})");
        }

        // Let's write a little info about the lobby we joined / created
        Debug.Log("Lobby info:\n" + JsonConvert.SerializeObject(currentLobby));

        // Let's add some new data for our player and update the lobby state
        // Players can update their own data
        loggedInPlayer.Data.Add("ExamplePublicPlayerData",
            new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, "Everyone can see this"));

        loggedInPlayer.Data.Add("ExamplePrivatePlayerData",
            new PlayerDataObject(PlayerDataObject.VisibilityOptions.Private, "Only the host sees this"));

        loggedInPlayer.Data.Add("ExampleMemberPlayerData",
            new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, "Only lobby members see this"));

        // Update the lobby
        currentLobby = await Lobbies.Instance.UpdatePlayerAsync(
            lobbyId: currentLobby.Id,
            playerId: loggedInPlayer.Id,
            options: new UpdatePlayerOptions()
            {
                Data = loggedInPlayer.Data
            });

        Debug.Log($"Updated lobby {currentLobby.Name} ({currentLobby.Id})");
        Debug.Log("Updated lobby info:\n" + JsonConvert.SerializeObject(currentLobby));

        // Let's poll for the lobby data again just to see what it looks like
        currentLobby = await Lobbies.Instance.GetLobbyAsync(currentLobby.Id);

        Debug.Log("Latest lobby info:\n" + JsonConvert.SerializeObject(currentLobby));

        if (!currentLobby.HostId.Equals(loggedInPlayer.Id))
        {
            // Since we're not the lobby host, let's just leave the lobby
            await Lobbies.Instance.RemovePlayerAsync(
                lobbyId: currentLobby.Id,
                playerId: loggedInPlayer.Id);

            Debug.Log($"Left lobby {currentLobby.Name} ({currentLobby.Id})");

            currentLobby = null;
        }
        else
        {
            // Only hosts can set lobby data, and we're the host, so let's set some
            // Note that lobby host can be passed around intentionally (by the current host updating the host id)
            // Host is randomly assigned if the previous host leaves

            // Let's update some existing lobby data
            currentLobby.Data["GameMode"] =
                new DataObject(DataObject.VisibilityOptions.Public, "deathmatch", DataObject.IndexOptions.S2);

            // Let's add some new data to the lobby
            currentLobby.Data.Add("ExamplePublicLobbyData",
                new DataObject(DataObject.VisibilityOptions.Public, "Everyone can see this"));

            currentLobby.Data.Add("ExamplePrivateLobbyData",
                new DataObject(DataObject.VisibilityOptions.Private, "Only the host sees this"));

            currentLobby.Data.Add("ExampleMemberLobbyData",
                new DataObject(DataObject.VisibilityOptions.Member, "Only lobby members see this"));

            // OK, now let's try to push these local changes to the service
            currentLobby = await Lobbies.Instance.UpdateLobbyAsync(
                lobbyId: currentLobby.Id,
                options: new UpdateLobbyOptions()
                {
                    Data = currentLobby.Data
                });

            // Let's print the updated lobby
            Debug.Log($"Updated lobby {currentLobby.Name} ({currentLobby.Id})");
            Debug.Log("Updated lobby info:\n" + JsonConvert.SerializeObject(currentLobby));

            // Since we're the host, let's wait a second and then heartbeat the lobby
            await Task.Delay(1000);
            await Lobbies.Instance.SendHeartbeatPingAsync(currentLobby.Id);

            // Let's print the updated lobby.  The LastUpdated time should be different.
            Debug.Log($"Heartbeated lobby {currentLobby.Name} ({currentLobby.Id})");
            Debug.Log("Updated lobby info:\n" + JsonConvert.SerializeObject(currentLobby));

            // OK, we're done with the lobby - let's delete it
            await Lobbies.Instance.DeleteLobbyAsync(currentLobby.Id);

            Debug.Log($"Deleted lobby {currentLobby.Name} ({currentLobby.Id})");

            currentLobby = null;
        }

        // Now, let's try the QuickJoin API, which just puts our player in a matching lobby automatically
        // This is fast and reliable (as long as matching lobbies are available), but removes some user
        //   interactivity (can't choose from a list of lobbies)
        // You can use filters to specify which types of lobbies can be joined just like a Query call
        // This also shows an example of how to catch lobby exceptions
        // Note that the QueryJoin API will throw exceptions on failures to find a matchmaking lobby,
        //   so it's more likely to fail than other API calls
        try
        {
            Debug.Log($"Trying to use Quick Join to find a lobby...");
            currentLobby = await Lobbies.Instance.QuickJoinLobbyAsync(new QuickJoinLobbyOptions
            {
                Player = loggedInPlayer, // Including the player here lets us join with data pre-populated
                Filter = new List<QueryFilter>
                {
                    // Let's search for lobbies with a specific name
                    new QueryFilter(
                        field: QueryFilter.FieldOptions.Name,
                        op: QueryFilter.OpOptions.EQ,
                        value: "My New Lobby"),

                    // You can add more filters here, such as filters on custom data fields
                }
            });
        }
        catch (LobbyServiceException ex)
        {
            if (ex.Reason == LobbyExceptionReason.NoOpenLobbies)
            {
                Debug.LogWarning("QuickJoin has failed because there are no lobbies to join. " +
                    "If you are running the HelloWorld sample for the first time, this is expected.\n" +
                    "Try using a second client to create a new lobby that matches the name filter above, then try to QuickJoin again.");
            }
            else
            {
                Debug.LogException(ex);
            }
        }

        // If we didn't find a lobby, abort run
        if (currentLobby == null)
        {
            Debug.Log("Unable to find a lobby using Quick Join");
            return;
        }

        // Let's write a little info about the lobby we quick-joined
        Debug.Log($"Joined lobby {currentLobby.Name} ({currentLobby.Id})");
        Debug.Log("Lobby info:\n" + JsonConvert.SerializeObject(currentLobby));

        // There's not anything else we can really do here, so let's leave the lobby
        await Lobbies.Instance.RemovePlayerAsync(
            lobbyId: currentLobby.Id,
            playerId: loggedInPlayer.Id);

        Debug.Log($"Left lobby {currentLobby.Name} ({currentLobby.Id})");
    }

    // Log in a player using Unity's "Anonymous Login" API and construct a Player object for use with the Lobbies APIs
    static async Task<Player> GetPlayerFromAnonymousLoginAsync()
    {
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            Debug.Log($"Trying to log in a player ...");

            // Use Unity Authentication to log in
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                throw new InvalidOperationException("Player was not signed in successfully; unable to continue without a logged in player");
            }
        }

        Debug.Log("Player signed in as " + AuthenticationService.Instance.PlayerId);

        // Player objects have Get-only properties, so you need to initialize the data bag here if you want to use it
        return new Player(AuthenticationService.Instance.PlayerId, null, new Dictionary<string, PlayerDataObject>());
    }
}
