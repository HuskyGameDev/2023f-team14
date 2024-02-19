using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;

[Serializable]
struct PoolObject
{
    public GameObject Prefab;
    public int PrespawnCount;
}
public class GameObjectPool : MonoBehaviour, IPumpingActionPool<GameObject>
{
    public static GameObjectPool Instance { get; private set; }

    [SerializeField]
    private List<PoolObject> PooledPrefabList;
    private readonly HashSet<GameObject> prefabs = new();
    private readonly Dictionary<GameObject, ObjectPool<GameObject>> pooledObjects = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        foreach (var obj in PooledPrefabList)
            RegisterPrefabInternal(obj.Prefab, obj.PrespawnCount);
    }

    void OnDestroy()
    {
        foreach (var prefab in prefabs)
        {
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
                Assert.IsNotNull(prefab.gameObject, "Non null game object in object pool");
        }
    }

    public GameObject GetObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        var gameObj = pooledObjects[prefab].Get();
        var goTransform = gameObj.transform;
        goTransform.SetPositionAndRotation(position, rotation);
        return gameObj;
    }

    public void ReturnObject(GameObject instance, GameObject prefab)
    {
        pooledObjects[prefab].Release(instance);
    }

    private void RegisterPrefabInternal(GameObject prefab, int prespawnCount)
    {
        GameObject CreateFunc() => Instantiate(prefab);
        void ActionOnGet(GameObject go) => go.SetActive(true);
        void ActionOnRelease(GameObject go) => go.SetActive(false);
        void ActionOnDestroy(GameObject go) => Destroy(go);
        prefabs.Add(prefab);

        pooledObjects[prefab] = new ObjectPool<GameObject>(CreateFunc, ActionOnGet, ActionOnRelease, ActionOnDestroy);

        var prespawnedObjects = new List<GameObject>();
        for (int i = 0; i < prespawnCount; i++)
            prespawnedObjects.Add(pooledObjects[prefab].Get());

        foreach (var go in prespawnedObjects)
            pooledObjects[prefab].Release(go);
    }
}


public interface IPumpingActionPool<T>
{
    /// <summary>
    /// Gets an instance of the given prefab from the pool. The prefab must be registered.
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <returns></returns>
    public abstract T GetObject(GameObject prefab, Vector3 position, Quaternion rotation);
    /// <summary>
    /// Return an object to the pool
    /// </summary>
    public abstract void ReturnObject(T instance, GameObject prefab);
}