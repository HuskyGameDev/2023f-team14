using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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

    public Image healthBar;
    private void Awake()
    {
        camera = GetComponentInChildren<Camera>();
        PlayerMovement = GetComponent<PlayerMovement>();
        GameObject canvas = GameObject.Find("Canvas");
        if(canvas != null){
            healthBar = canvas.transform.Find("HUD_Health_GreenBackground").GetComponent<Image>();
        }
    }

    public override void OnNetworkSpawn()
    {
        //if is client
        if(IsOwner){
            health.OnValueChanged += (float oldv, float newv) =>
            {
                healthBar.fillAmount -= .1f;
            };
        }
        if (IsServer)
        {
            team = new(Team.NoTeam);
            health.OnValueChanged += (float oldv, float newv) =>
            {
                //if (newv < 0) { DieServerRpc(); return; }
                if (newv < 0) { ScoreKeeper.Instance.SpawnPlayer(this); return; }
                if (newv > maxHealth) { health.Value = maxHealth; }
            };
        }
        base.OnNetworkSpawn();
    }

    // SERVER SIDE ONLY
    public void Hit(ulong assailant, float damage)
    {
        Debug.Log(assailant + " hit " + OwnerClientId + " for " + damage + " damage!");
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
    public void Respawn(Vector3 spawnPosition)
    {
        Debug.Log("spawnPos: " + spawnPosition);
        ResetPositionServerRpc(spawnPosition);
        health.Value = maxHealth;
        //healthBar.fillAmount = 1f;
    }

    [ServerRpc]
    private void UpdateHealthBarServerRpc(ulong playerID)
    {
        ClientRpcParams clientRpcParams = new ClientRpcParams{
            Send = new ClientRpcSendParams{
                TargetClientIds = new ulong[]{playerID}
            }
        };
        UpdateHealthBarClientRpc(clientRpcParams);
    }

    [ClientRpc]
    private void UpdateHealthBarClientRpc(ClientRpcParams clientRpcParams = default){
        if(!IsOwner) return;
        if (healthBar != null)
            {
                //Set healthBar to be current health
                healthBar.fillAmount = health.Value / 100;
            }
    }
}
