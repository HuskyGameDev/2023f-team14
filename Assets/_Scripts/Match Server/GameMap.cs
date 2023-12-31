using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "MapData")]
public class GameMap : ScriptableObject
{
    [SerializeField]
    public string Name;
    [SerializeField]
    private SpawnPoint spawnPointPrefab;
    //TODO Track spawn points and objective locations.
    [SerializeField]
    private Vector3[] spawnPointLocations;

    public SpawnPoint[] spawnPoints;
    public bool Initialized { get; private set; }

    private void Awake()
    {
    }

    public void Initialize(GameMode gm, int numTeams)
    {
        spawnPoints = new SpawnPoint[spawnPointLocations.Length];
        for (int i = 0; i < spawnPointLocations.Length; i++)
        {
            spawnPoints[i] = Instantiate(spawnPointPrefab);
            spawnPoints[i].transform.position = spawnPointLocations[i];
        }

        TagPoints(gm, numTeams);

        Initialized = true;
    }

    public void DespawnSpawnPoints()
    {
        Debug.Log("Despawning spawn points...");
        foreach (SpawnPoint point in spawnPoints)
            Destroy(point);
    }

    public void TagPoints(GameMode gm)
    {
        TagPoints(gm, gm.MinNumTeams);
    }

    public void TagPoints(GameMode gm, int numTeams)
    {
        numTeams = Max(numTeams, gm.MinNumTeams);
        switch (gm.Algo)
        {
            case SpawnPointTaggingAlgorithm.ShardSplit:
                ShardSplit(numTeams);
                break;
            case SpawnPointTaggingAlgorithm.AroundPositions:
                //TODO: update with map-specific transforms from gm
                Vector3[] transforms = new Vector3[gm.prefabs.Count];
                for (int i = 0; i < gm.prefabs.Count; i++)
                {
                    transforms[i] = gm.prefabs[i].position;
                }
                AroundPositions(transforms, numTeams);
                break;
            default: //Default to FFA spawn points
                foreach (SpawnPoint p in spawnPoints)
                    p.teamTagMask = (ushort)Team.NoTeam;
                break;
        }
    }

    public List<SpawnPoint> GetValidSpawnPoints(Team team)
    {
        List<SpawnPoint> list = new();
        foreach (SpawnPoint sp in spawnPoints)
        {
            //If the spawn point is for everyone or our team, it's valid
            if (sp.teamTagMask == (ushort)Team.NoTeam || (sp.teamTagMask & (ushort)team) != 0) list.Add(sp);
        }
        return list;
    }

    /// <summary>
    /// This algorithm splits the points into roughly equal sets in nearby areas. Each set is tagged for only one team.
    /// </summary>
    /// <param name="points">All spawn points for this map.</param>
    private void ShardSplit(int numTeams)
    {
        int numSpawnPoints = spawnPoints.Length;
        var centralPointPos = Vector3.zero;
        foreach (SpawnPoint p in spawnPoints)
        {
            centralPointPos += p.transform.position;
        }
        centralPointPos = new Vector3(centralPointPos.x / (float)numSpawnPoints, centralPointPos.y / (float)numSpawnPoints, centralPointPos.z / (float)numSpawnPoints);
        //TODO From the central point, slice into numTeams sections
    }

    /// <summary>
    /// This algorithm groups points based on linear distance from a given position. Each point is tagged for only one team. Only the first numTeams positions will be read.
    /// </summary>
    /// <param name="points">All spawn points on the map.</param>
    /// <param name="positions">An array with the first numTeams elements being positions of ordered, team-specific transforms.</param>
    /// <param name="numTeams">The number of teams in game.</param>
    private void AroundPositions(Vector3[] positions, int numTeams)
    {
        if (spawnPoints.Length == 0 || positions.Length == 0) return;
        int i, j, shortestIndex;
        Vector3 pos;
        int len = spawnPoints.Length;
        float dist, shortest;

        for (i = 0; i < len; i++)
        {
            spawnPoints[i].teamTagMask = 0;
            shortestIndex = 0;
            pos = positions[i % numTeams];
            shortest = float.MaxValue;
            for (j = i; j < len; j++)
            {
                //Debug.Log(spawnPoints[j].transform.position);
                dist = Vector3.Distance(spawnPoints[j].transform.position, pos);
                if (dist < shortest)
                {
                    shortest = dist;
                    shortestIndex = j;
                }
            }
            (spawnPoints[i], spawnPoints[shortestIndex]) = (spawnPoints[shortestIndex], spawnPoints[i]); //This is just fancy notation for swap
            spawnPoints[i].teamTagMask += (ushort)Math.Pow(2, i % numTeams);
        }

        /*
        *   This approach is naive. This will map points that are closest to positions to that team, 
        *   with no regard for fairness.
        int i;
        float shortest;
        int shortestIndex = -1;
        float dist;
        foreach (SpawnPoint point in points)
        {
            shortest = float.MaxValue;
            for (i = 0; i < numTeams; i++)
            {
                dist = Vector3.Distance(positions[i], point.transform.position);
                if (dist < shortest)
                {
                    shortest = dist;
                    shortestIndex = i;
                }
            }

            point.teamTagMask = (ushort)Math.Pow(2, shortestIndex);
        }
        */
    }

    private int Max(int a, int b)
    {
        if (a >= b) return a;
        return b;
    }
}
