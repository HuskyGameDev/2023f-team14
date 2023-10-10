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

    [Header("Player Data")]
    [SerializeField]
    private int respawnTimeMilliseconds = DefaultRespawnTimeMilliseconds;
    public TimeSpan RespawnTime => TimeSpan.FromSeconds(respawnTimeMilliseconds);

    [SerializeField]
    private int playerHealth = DefaultPlayerHealth;
    public int PlayerHealth => playerHealth;

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
    [SerializeField]
    private GameObject[] prefabs;


}

