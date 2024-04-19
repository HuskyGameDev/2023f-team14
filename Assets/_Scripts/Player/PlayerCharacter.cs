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
    public InGameUI HUD;
    public float maxHealth;
    public new Camera camera;
    [DoNotSerialize]
    public PlayerMovement PlayerMovement { get; private set; }

    public Action<PlayerCharacter> OnDeath;
    public Action<PlayerCharacter> OnSpawn;
    private ulong id;
    [DoNotSerialize]
    public NetworkVariable<float> respawnTime;
    public bool ActiveIFrames => respawnTime.Value + 1f > Time.time;

    private void Awake()
    {
        camera = GetComponentInChildren<Camera>();
        PlayerMovement = GetComponent<PlayerMovement>();
        if (IsOwner) HUD = FindObjectOfType<InGameUI>();
        respawnTime = new()
        {
            Value = 0
        };
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            team = new(Team.NoTeam, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
            health.OnValueChanged += (float oldv, float newv) =>
            {
                if (ActiveIFrames)
                {
                    if (newv != maxHealth)
                    {
                        health.Value = maxHealth;
                    }
                    return;
                }
                if (newv <= 0) { Kill(); return; }
                if (newv > maxHealth) { health.Value = maxHealth; }
            };
        }
        if (IsOwner)
        {
            health.OnValueChanged += (float oldv, float newv) =>
            {
                if (!HUD) HUD = FindObjectOfType<InGameUI>();
                HUD.UpdateHealth(newv, maxHealth);
            };
        }

        OnSpawn?.Invoke(this);

        base.OnNetworkSpawn();
    }

    // SERVER SIDE ONLY
    public void Hit(ulong assailant, float damage)
    {
        if (!IsServer)
            throw new MethodAccessException("This method should not be called by clients!");
        //Debug.Log(assailant + " hit " + OwnerClientId + " for " + damage + " damage!");
        if (ActiveIFrames) return;
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

    //SERVER ONLY
    public void RespawnIFramesStart()
    {
        if (!IsServer) throw new MethodAccessException("This method should not be called by clients!");
        respawnTime.Value = Time.time;
        health.Value = maxHealth;
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
        OnSpawn?.Invoke(this);
    }
}
