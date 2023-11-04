using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
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

    private SpawnPoint[] spawnPoints;

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

        TagPoints(spawnPoints, gm, numTeams);
    }

    public void TagPoints(SpawnPoint[] points, GameMode gm)
    {
        TagPoints(points, gm, gm.MinNumTeams);
    }

    public void TagPoints(SpawnPoint[] points, GameMode gm, int numTeams)
    {
        switch (gm.Algo)
        {
            case SpawnPointTaggingAlgorithm.ShardSplit:
                ShardSplit(points, numTeams);
                break;
            case SpawnPointTaggingAlgorithm.AroundPositions:
                //TODO: update with map-specific transforms from gm
                Vector3[] transforms = new Vector3[gm.prefabs.Length];
                for (int i = 0; i < gm.prefabs.Length; i++)
                {
                    transforms[i] = gm.prefabs[i].transform.position;
                }
                AroundPositions(points, transforms, numTeams);
                break;
            default: //Default to FFA spawn points
                foreach (SpawnPoint p in points)
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
    private void ShardSplit(SpawnPoint[] points, int numTeams)
    {
        int numSpawnPoints = points.Length;
        var centralPointPos = Vector3.zero;
        foreach (SpawnPoint p in points)
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
    private void AroundPositions(SpawnPoint[] points, Vector3[] positions, int numTeams)
    {
        if (numTeams == 0 || points.Length == 0 || positions.Length == 0) return;

        int i, j, shortestIndex;
        Vector3 pos;
        int len = points.Length;
        float dist, shortest;

        for (i = 0; i < len; i++)
        {
            shortestIndex = 0;
            pos = positions[i % numTeams];
            shortest = float.MaxValue;
            for (j = i; j < len; j++)
            {
                dist = Vector3.Distance(points[j].transform.position, pos);
                if (dist < shortest)
                {
                    shortest = dist;
                    shortestIndex = j;
                }
            }
            (points[i], points[shortestIndex]) = (points[shortestIndex], points[i]); //This is just fancy notation for swap
            points[i].teamTagMask += (ushort)Math.Pow(2, i % numTeams);
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
}
