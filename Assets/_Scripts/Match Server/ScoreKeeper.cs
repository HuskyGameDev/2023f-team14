using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class ScoreKeeper : NetworkBehaviour
{
    public static ScoreKeeper Instance;
    [SerializeField]
    private GameMode gameMode;
    [SerializeField]
    private GameMap gameMap;
    [SerializeField]
    private Button startButton;

    public List<int> teamCounts;


    private readonly Dictionary<ulong, PlayerCharacter> connectedPlayers = new();
    private readonly HashSet<ulong> readiedPlayers = new();
    public int NumTeams => teamCounts.Count;
    public NetworkVariable<bool> GameInProgress = new();
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
            StartGameServerRpc();
        });

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
        base.OnNetworkSpawn();
        if (IsServer)
        {
            ConnectionNotificationManager.Instance.OnClientConnectionNotification += ClientConnectionChange;
            NetworkManager.Singleton.OnServerStopped += EndGame;
            GameInProgress.Value = false;
        }
        if (!IsHost) startButton.enabled = false;
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            ConnectionNotificationManager.Instance.OnClientConnectionNotification -= ClientConnectionChange;
            NetworkManager.Singleton.OnServerStopped -= EndGame;
            EndGame(true);
        }

        //EndGame(true);
        base.OnNetworkDespawn();
    }

    private void EndGame(bool b = true)
    {
        gameMap.DespawnSpawnPoints();
        connectedPlayers.Clear();
        teamCounts.Clear();
    }

    public override void OnDestroy()
    {
        if (!IsServer) EndGame(true);
        base.OnDestroy();
    }

    [ServerRpc]
    public void StartGameServerRpc()
    {

        //TODO: Scene Changes for clients
        Debug.Log("Initializing " + gameMode.Name + " on " + gameMap.Name);
        gameMap.Initialize(gameMode, NumTeams);
        gameMode.Initialize();
        RerackTeams();

        GameInProgress.Value = true;
    }
    // override OnDestroy if needed

    private void ClientConnectionChange(ulong id, ConnectionNotificationManager.ConnectionStatus status)
    {
        if (!GameInProgress.Value) return;
        switch (status)
        {
            case ConnectionNotificationManager.ConnectionStatus.Connected:
                AddPlayer(id);
                break;
            case ConnectionNotificationManager.ConnectionStatus.Disconnected:
                RemovePlayer(id);
                break;
            default:
                Debug.LogError("Player " + id + " connection status not found!");
                break;
        }
        gameMap.TagPoints(gameMode);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id">The player's unique id</param>
    /// <param name="teamCounts">A list, indexed by team number, containing the number of players on each team</param>
    /// <returns>The team that the player is assigned to.</returns>
    private Team AddPlayer(ulong id)
    {
        var pc = NetworkManager.Singleton.ConnectedClients[id].PlayerObject.GetComponent<PlayerCharacter>();
        if (!connectedPlayers.TryAdd(id, pc))
        {
            Debug.LogError("Cannot add player " + id + " to game!");
            return Team.NoTeam;
        }
        var newTeam = gameMode.AssignNewPlayer(teamCounts);
        pc.team.Value = newTeam;
        teamCounts = GetTeamCounts();
        return newTeam;
    }

    private void RemovePlayer(ulong id)
    {
        connectedPlayers.Remove(id);
        teamCounts = GetTeamCounts();
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
        int team;
        foreach (var pc in connectedPlayers.Values)
        {
            team = TeamUtils.GetTeamNumber(pc.team.Value);

            for (int i = list.Count; i < team + 1; i++)
                list.Add(0);

            list[team]++;
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
        pc.health.Value = pc.maxHealth;

        if (!GameInProgress.Value && !gameMap.Initialized)
        {
            gameMap.Initialize(gameMode, NumTeams);
        }

        //!This call is temporary. Remove when PlayerCharacter.DieServerRpc is implemented
        pc.OnDeath?.Invoke(pc);

        pc.PlayerMovement.SetPosition(gameMode.CalculateSpawnPoint(gameMap, pc.team.Value));
    }

    private void RerackTeams()
    {
        connectedPlayers.Clear();
        teamCounts = new();
        var clients = NetworkManager.Singleton.ConnectedClients;
        Debug.Log("Reracking teams for " + clients.Count + " players...");
        foreach (KeyValuePair<ulong, NetworkClient> pair in clients)
        {
            var res = AddPlayer(pair.Key);
            if (TeamUtils.GetTeamNumber(res) >= teamCounts.Count)
            {
                teamCounts.Add(1);
                continue;
            }
            teamCounts[TeamUtils.GetTeamNumber(res)]++;
        }
    }
}