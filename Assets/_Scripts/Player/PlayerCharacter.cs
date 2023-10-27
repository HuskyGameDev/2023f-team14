using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerCharacter : Unity.Netcode.NetworkBehaviour
{
    public NetworkVariable<float> health;
    public NetworkVariable<int> score;
    public NetworkVariable<uint> team;
    public float maxHealth;
    public new Camera camera;

    private void Awake()
    {
        health.OnValueChanged += (float oldv, float newv) =>
        {
            if (newv < 0) { DieServerRpc(); return; }
            if (newv > maxHealth) { health.Value = maxHealth; }
        };
        camera = GetComponentInChildren<Camera>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }

    // SERVER SIDE ONLY
    public void Hit(ulong assailant, float damage)
    {
        health.Value -= damage;
    }

    [ServerRpc]
    private void DieServerRpc()
    {
        throw new NotImplementedException();
    }
}
