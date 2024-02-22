using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;
public class NetworkObjectPool : NetworkBehaviour, IPumpingActionPool<NetworkObject>
{
    public static NetworkObjectPool Instance { get; private set; }

    [SerializeField]
    private List<PoolObject> PooledPrefabList;
    private readonly HashSet<GameObject> prefabs = new();
    private readonly Dictionary<GameObject, ObjectPool<NetworkObject>> pooledObjects = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;

    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        foreach (var obj in PooledPrefabList)
            RegisterPrefabInternal(obj.Prefab, obj.PrespawnCount);
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        foreach (var prefab in prefabs)
        {
            NetworkManager.Singleton.PrefabHandler.RemoveHandler(prefab);
            pooledObjects[prefab].Clear();
        }
        pooledObjects.Clear();
        prefabs.Clear();
    }

    public void OnValidate()
    {
        for (int i = 0; i < PooledPrefabList.Count; i++)
        {
            var prefab = PooledPrefabList[i].Prefab;
            if (prefab != null)
                Assert.IsNotNull(prefab.GetComponent<NetworkObject>(), "Non null network object in object pool");
        }
    }

    public NetworkObject GetObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        var netObj = pooledObjects[prefab].Get();
        var noTransform = netObj.transform;
        noTransform.SetPositionAndRotation(position, rotation);
        return netObj;
    }

    public void ReturnObject(NetworkObject netObj, GameObject prefab)
    {
        pooledObjects[prefab].Release(netObj);
    }

    private void RegisterPrefabInternal(GameObject prefab, int prespawnCount)
    {
        NetworkObject CreateFunc() => Instantiate(prefab).GetComponent<NetworkObject>();
        void ActionOnGet(NetworkObject no) => no.gameObject.SetActive(true);
        void ActionOnRelease(NetworkObject no) => no.gameObject.SetActive(false);
        void ActionOnDestroy(NetworkObject no) => no.Despawn(true);
        prefabs.Add(prefab);

        pooledObjects[prefab] = new ObjectPool<NetworkObject>(CreateFunc, ActionOnGet, ActionOnRelease, ActionOnDestroy);

        var prespawnedObjects = new List<NetworkObject>();
        for (int i = 0; i < prespawnCount; i++)
            prespawnedObjects.Add(pooledObjects[prefab].Get());

        foreach (var no in prespawnedObjects)
            pooledObjects[prefab].Release(no);

        NetworkManager.Singleton.PrefabHandler.AddHandler(prefab, new PooledPrefabInstanceHandler(prefab, this));
    }

}

class PooledPrefabInstanceHandler : INetworkPrefabInstanceHandler
{
    GameObject prefab;
    NetworkObjectPool pool;
    public PooledPrefabInstanceHandler(GameObject prefab, NetworkObjectPool pool)
    {
        this.prefab = prefab;
        this.pool = pool;
    }

    public void Destroy(NetworkObject networkObject)
    {
        pool.ReturnObject(networkObject, prefab);
    }

    public NetworkObject Instantiate(ulong ownerClientId, Vector3 position, Quaternion rotation)
    {
        return pool.GetObject(prefab, position, rotation);
    }
}