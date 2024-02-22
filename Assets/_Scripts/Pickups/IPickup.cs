using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public interface IPickup
{
    //SERVER ONLY
    public void PickUp(PlayerCharacter player);
    public void PickUpServerRpc(ServerRpcParams serverRpcParams = default);
}

public abstract class NetworkPickup : NetworkBehaviour, IPickup
{
    public string prefabName;
    public Action<NetworkPickup> OnPickup;
    public abstract void PickUp(PlayerCharacter player);

    protected GameObject prefab;

    [ServerRpc(RequireOwnership = false)]
    public void PickUpServerRpc(ServerRpcParams serverRpcParams = default)
    {
        PickUp(NetworkManager.Singleton.ConnectedClients[serverRpcParams.Receive.SenderClientId].PlayerObject.GetComponent<PlayerCharacter>());
        OnPickup?.Invoke(this);
        NetworkObjectPool.Instance.ReturnObject(NetworkObject, prefab);
    }

    private void Start()
    {
        prefab = (GameObject)Resources.Load(prefabName);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;
        if (other.CompareTag("Player"))
        {
            OnPickup?.Invoke(this);
            PickUp(other.GetComponent<PlayerCharacter>());
            NetworkObjectPool.Instance.ReturnObject(NetworkObject, prefab);
        }
    }
}

public abstract class NetworkPickupRespawnable : NetworkPickup
{
    public new Action<NetworkPickupRespawnable> OnPickup;
    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;
        if (other.CompareTag("Player"))
        {
            OnPickup?.Invoke(this);
            PickUp(other.GetComponent<PlayerCharacter>());
            NetworkObjectPool.Instance.ReturnObject(NetworkObject, prefab);
            NetworkObject.Despawn(false);
        }
    }
}