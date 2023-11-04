using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[CreateAssetMenu(menuName = "GameMode")]
public class GameMode : ScriptableObject
{
    private const int DefaultPlayerHealth = 100;
    private const int DefaultRespawnTimeMilliseconds = 5000;

    [SerializeField]
    private string gm_name;
    public string Name => gm_name;
    [Header("Player Data")]
    [SerializeField]
    private int respawnTimeMilliseconds = DefaultRespawnTimeMilliseconds;
    public TimeSpan RespawnTime => TimeSpan.FromSeconds(respawnTimeMilliseconds);

    [SerializeField]
    private int playerHealth = DefaultPlayerHealth;
    public int PlayerHealth => playerHealth;

    [SerializeField]
    public SpawnPointTaggingAlgorithm Algo;

    /* */

    [Header("Team Data")]
    [SerializeField]
    private int maxTeamSize;
    public int MaxTeamSize => maxTeamSize;

    [SerializeField]
    private int minNumTeams;
    public int MinNumTeams => minNumTeams;

    /* */

    [Header("Objective Data")]
    public NetworkObject[] prefabs;


    /* */

    private Unity.Mathematics.Random random;
    public void Initialize()
    {
        random = new Unity.Mathematics.Random();
        foreach (NetworkObject prefab in prefabs)
        {
            NetworkObject o = Instantiate(prefab);
            o.Spawn();
        }
    }

    /// <summary>
    /// This assignment algorithm will place one player per team until the minimum number of teams has been reached.
    /// Afterward, it will assign  each player to the team with the fewest players, and will only create another team 
    /// if all existing teams are full. This method may be overridden in subclasses to change its behaviour.
    /// </summary>
    /// <param name="teamCounts">An array, indexed by team number, containing the number of players on each team</param>
    /// <returns>The team to assign the next player to connect to.</returns>
    public Team AssignNewPlayer(List<int> teamCounts)
    {
        //If there are fewer current teams than the required minimum number. Create a new team.
        if (teamCounts.Count < minNumTeams)
            return TeamUtils.TeamNumberToTeam(teamCounts.Count);

        for (int teamNumber = 0; teamNumber < teamCounts.Count; teamNumber++)
        {
            //This team has room for at least one more player. Assign to this team.
            if (teamCounts[teamNumber] < maxTeamSize) return TeamUtils.TeamNumberToTeam(teamNumber);
        }

        //All current teams are full. Create a new team.
        return TeamUtils.TeamNumberToTeam(teamCounts.Count);
    }

    /// <summary>
    /// select any point for our team that an enemy isn't near.
    /// </summary>
    /// <param name="map">The map we're playing on</param>
    /// <param name="playerTeam">The team of the spawning player</param>
    /// <returns></returns>
    public Vector3 CalculateSpawnPoint(GameMap map, Team playerTeam)
    {
        var points = map.GetValidSpawnPoints(playerTeam);
        points.RemoveAll((p) => p.IsEnemyPresent);

        //randomly select any point for our team that an enemy isn't in
        return points[random.NextInt(points.Count)].transform.position;
    }
}

public enum SpawnPointTaggingAlgorithm : ushort
{
    NoTags,
    ShardSplit,

    AroundPositions
}