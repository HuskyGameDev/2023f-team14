using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerCharacter : Unity.Netcode.NetworkBehaviour
{
    public NetworkVariable<float> health;
    public NetworkVariable<int> score;
    public NetworkVariable<Team> team;
    public float maxHealth;
    public new Camera camera;

    private void Awake()
    {

        camera = GetComponentInChildren<Camera>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            team = new(Team.NoTeam);
            health.OnValueChanged += (float oldv, float newv) =>
            {
                //if (newv < 0) { DieServerRpc(); return; }
                if (newv < 0) { ScoreKeeper.Instance.SpawnPlayerServerRpc(); return; }
                if (newv > maxHealth) { health.Value = maxHealth; }
            };
        }
        base.OnNetworkSpawn();
    }

    // SERVER SIDE ONLY
    public void Hit(ulong assailant, float damage)
    {
        Debug.Log(assailant + " hit me for " + damage + " damage!");
        health.Value -= damage;
    }

    [ServerRpc]
    private void DieServerRpc()
    {
        //TODO: Implement
    }

    [ServerRpc]
    private void ResetPositionServerRpc(Vector3 pos)
    {
        transform.position = pos;
    }

    //SERVER ONLY
    public void Respawn(Vector3 spawnPosition)
    {
        Debug.Log("spawnPos: " + spawnPosition);
        ResetPositionServerRpc(spawnPosition);
        health.Value = maxHealth;
    }
}
