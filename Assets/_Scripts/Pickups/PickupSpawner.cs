using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PickupSpawner : NetworkBehaviour
{
    [SerializeField]
    private NetworkPickupRespawnable prefab;
    [SerializeField]
    private float respawnTimeSeconds;
    [SerializeField]
    private bool startSpawned = true;
    private bool spawned;
    private float pickupTime = Mathf.NegativeInfinity;
    private float ServerTime => (float)NetworkManager.ServerTime.Time;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer)
        {
            if (startSpawned) pickupTime = 1 - respawnTimeSeconds;
        }
    }

    public void OnPickup(NetworkPickupRespawnable pickup)
    {
        spawned = false;
        pickupTime = ServerTime;
        pickup.OnPickup -= OnPickup;
    }

    private void Update()
    {
        if (!IsServer) return;
        if (!spawned && pickupTime + respawnTimeSeconds <= ServerTime)
        {
            Spawn();
        }
    }

    private void Spawn()
    {
        spawned = true;
        var no = NetworkObjectPool.Instance.GetObject(prefab.gameObject, transform.position, transform.rotation);
        no.GetComponent<NetworkPickupRespawnable>().OnPickup += OnPickup;
        no.SpawnWithObservers = true;
        if (!no.IsSpawned) no.Spawn();
    }
}
