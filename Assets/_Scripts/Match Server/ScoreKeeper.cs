using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class ScoreKeeper : NetworkBehaviour
{
    public static ScoreKeeper Instance;
    //TODO: update gameMode and gameMap across network
    [SerializeField]
    private GameMode gameMode;
    [SerializeField]
    private GameMap gameMap;
    [SerializeField]
    private Button startButton;
    public bool IsGameInProgress;

    [SerializeField]
    private List<int> teamCounts;

    private Dictionary<ulong, PlayerCharacter> connectedPlayers = new();
    private HashSet<ulong> readiedPlayers = new HashSet<ulong>();
    public int numTeams => teamCounts.Count;
    void Awake()
    {
        if (Instance != null)
        {
            throw new Exception($"Detected more than one instance of {nameof(ScoreKeeper)}! " +
                $"Do you have more than one component attached to a {nameof(GameObject)}?");
        }
        Instance = this;

        startButton.onClick.AddListener(() =>
        {
            StartGame(gameMode, gameMap);
        });

        IsGameInProgress = false;
        teamCounts = new();
    }

    void Start()
    {
    }

    void Update()
    {
    }


    public override void OnNetworkSpawn()
    {
        if (IsServer)
            ConnectionNotificationManager.Instance.OnClientConnectionNotification += ClientConnectionChange;
        if (!(IsHost || IsServer)) startButton.enabled = false;
        base.OnNetworkSpawn();
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
            ConnectionNotificationManager.Instance.OnClientConnectionNotification -= ClientConnectionChange;
        base.OnNetworkDespawn();
    }

    public void StartGame(GameMode gm, GameMap map)
    {
        if (!IsServer) return;

        //Scene Changes for clients

        gameMode = gm;
        gameMap = map;
        RerackTeams();
        gameMap.Initialize(gm, numTeams);

        Debug.Log("Initializing " + gameMode.Name + " on " + gameMap.Name);
        gameMode.Initialize();
    }
    // override OnDestroy if needed

    private void ClientConnectionChange(ulong id, ConnectionNotificationManager.ConnectionStatus status)
    {
        if (!IsGameInProgress) return;

        switch (status)
        {
            case ConnectionNotificationManager.ConnectionStatus.Connected:
                AddPlayer(id, GetTeamCounts());
                break;
            case ConnectionNotificationManager.ConnectionStatus.Disconnected:
                RemovePlayer(id);
                break;
            default:
                Debug.LogError("Player " + id + " connection status not found!");
                break;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id">The player's unique id</param>
    /// <param name="teamCounts">A list, indexed by team number, containing the number of players on each team</param>
    /// <returns>The team that the player is assigned to.</returns>
    private Team AddPlayer(ulong id, List<int> teamCounts)
    {
        var pc = NetworkManager.Singleton.ConnectedClients[id].PlayerObject.GetComponent<PlayerCharacter>();
        if (!connectedPlayers.TryAdd(id, pc))
        {
            Debug.LogError("Cannot add player " + id + " to game!");
            return Team.NoTeam;
        }
        var newTeam = gameMode.AssignNewPlayer(teamCounts);
        pc.team.Value = newTeam;
        return newTeam;
    }

    private void RemovePlayer(ulong id)
    {
        connectedPlayers.Remove(id);
    }

    /// <summary>
    /// Returns an array containing the number of players on each team. The length of this array will always be 
    /// equal to numTeams.
    /// 
    /// If no game is active or there are no connected players to a game, the array will be empty
    /// </summary>
    /// <returns>an array containing the number of players on each team, indexed by team number.</returns>
    private List<int> GetTeamCounts()
    {
        List<int> list = new();
        if (numTeams == 0 || !IsGameInProgress) return list;

        for (int i = 0; i < numTeams; i++)
        {
            list.Add(0);
        }

        foreach (var pc in connectedPlayers.Values)
        {
            list[TeamUtils.GetTeamNumber(pc.team.Value)]++;
        }
        return list;
    }

    [ServerRpc(RequireOwnership = false)]
    public void ReadyUpServerRpc(bool isReady, ServerRpcParams serverRpcParams = default)
    {
        var id = serverRpcParams.Receive.SenderClientId;
        if (isReady)
        {
            if (readiedPlayers.Add(id))
                return;
            Debug.LogWarning("Player " + id + " is already readied up!");
            return;
        }
        if (readiedPlayers.Remove(id))
            return;
        Debug.LogWarning("Player " + id + " is already not readied!");
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnPlayerServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var playerId = serverRpcParams.Receive.SenderClientId;
        var pc = NetworkManager.Singleton.ConnectedClients[playerId].PlayerObject.GetComponent<PlayerCharacter>();
        pc.Respawn(gameMode.CalculateSpawnPoint(gameMap, pc.team.Value));
    }

    private void RerackTeams()
    {
        connectedPlayers.Clear();
        teamCounts = new();
        var clients = NetworkManager.Singleton.ConnectedClients;
        Debug.Log("Reracking teams for " + clients.Count + " players...");
        foreach (KeyValuePair<ulong, NetworkClient> pair in clients)
        {
            var res = AddPlayer(pair.Key, teamCounts);
            if (TeamUtils.GetTeamNumber(res) >= teamCounts.Count)
            {
                teamCounts.Add(1);
                continue;
            }
            teamCounts[TeamUtils.GetTeamNumber(res)]++;
        }
    }
}