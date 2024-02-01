using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCharacter : Unity.Netcode.NetworkBehaviour
{
    [DoNotSerialize]
    public NetworkVariable<float> health;
    [DoNotSerialize]
    public NetworkVariable<int> score;
    [DoNotSerialize]
    public NetworkVariable<Team> team;
    public float maxHealth;
    public new Camera camera;
    public PlayerMovement PlayerMovement { get; private set; }

    public Action<PlayerCharacter> OnDeath;
    private ulong id;

    private void Awake()
    {
        camera = GetComponentInChildren<Camera>();
        PlayerMovement = GetComponent<PlayerMovement>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            team = new(Team.NoTeam);
            health.OnValueChanged += (float oldv, float newv) =>
            {
                //if (newv < 0) { DieServerRpc(); return; }
                if (newv < 0) { Kill(); return; }
                if (newv > maxHealth) { health.Value = maxHealth; }
            };
        }
        base.OnNetworkSpawn();
    }

    // SERVER SIDE ONLY
    public void Hit(ulong assailant, float damage)
    {
        if (!IsServer)
            throw new MethodAccessException("This method should not be called by clients!");
        //Debug.Log(assailant + " hit " + OwnerClientId + " for " + damage + " damage!");
        health.Value -= damage;
    }

    [ServerRpc]
    private void DieServerRpc()
    {
        OnDeath?.Invoke(this);
        //TODO: Implement
    }

    [ServerRpc]
    private void ResetPositionServerRpc(Vector3 pos)
    {
        transform.position = pos;
    }

    public void Kill()
    {
        if (!IsServer)
            throw new MethodAccessException("This method should not be called by clients!");
        OnDeath?.Invoke(this);
        ScoreKeeper.Instance.SpawnPlayer(this);
    }

    public void Respawn(Vector3 spawnPosition)
    {
        if (!IsServer)
            throw new MethodAccessException("This method should not be called by clients!");
        //Debug.Log("spawnPos: " + spawnPosition);
        ResetPositionServerRpc(spawnPosition);
        health.Value = maxHealth;
    }
}
