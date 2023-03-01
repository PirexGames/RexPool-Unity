using UnityEngine;
using UnityEngine.Pool;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GameObjectPoolSO", menuName = "PoolSO/GameObjectPoolSO")]
public class GameObjectPoolSO : ScriptableObject, IDisposable
{
    private Transform despawnParent;
    [SerializeField] private PoolType poolType;
    Dictionary<string, GameObjectPool> _dicPool = new Dictionary<string, GameObjectPool>(5);

    public void SetUpDespawnParent(Transform transform)
    {
        despawnParent = transform;
    }

    public GameObject Spawn(GameObject gameObject)
    {
        if (gameObject == null)
            return null;
        if (!_dicPool.TryGetValue(gameObject.name, out var pool))
        {
            pool = new GameObjectPool(gameObject, poolType, despawnParent);
            _dicPool.Add(gameObject.name, pool);
        }
        return pool.Spawn();
    }

    public GameObject Spawn(GameObject gameObject, Transform parent)
    {
        if (!gameObject)
            return null;
        var go = Spawn(gameObject);
        go.transform.SetParent(parent, false);
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = gameObject.transform.localScale;
        return go;
    }

    public void DeSpawn(GameObject gameObject)
    {
        if (_dicPool.TryGetValue(gameObject.name, out var pool))
        {
            pool.DeSpawn(gameObject);
        }
        else
        {
            Debug.LogWarning("gameObject not Pooled");
            GameObject.Destroy(gameObject);
        }
    }

    public void Dispose()
    {
        despawnParent = null;
        foreach (var pool in _dicPool.Values)
            pool.Dispose();
        _dicPool.Clear();
    }
}

internal class GameObjectPool : IDisposable
{
    private Transform _inactiveContainer;
    private GameObject _rootGameObject;
    private PoolType _poolType;

    public GameObjectPool(GameObject rootGameObject, PoolType poolType, Transform inactiveContainer = null)
    {
        _poolType = poolType;
        _rootGameObject = rootGameObject;
        _inactiveContainer = inactiveContainer;
    }

    private IObjectPool<GameObject> _pool;

    public IObjectPool<GameObject> Pool
    {
        get
        {
            if (_pool == null)
            {
                if (_poolType == PoolType.Stack)
                    _pool = new ObjectPool<GameObject>(GetRootEffect, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject);
                else
                    _pool = new LinkedPool<GameObject>(GetRootEffect, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject);
            }
            return _pool;
        }
    }

    private GameObject GetRootEffect()
    {
        var gameObject = GameObject.Instantiate(_rootGameObject);
        gameObject.name = _rootGameObject.name;
        return gameObject;
    }

    private void OnReturnedToPool(GameObject gameObject)
    {
        if (_inactiveContainer)
            gameObject.transform.SetParent(_inactiveContainer);
        else
            gameObject.transform.SetParent(null);
        gameObject.transform.localPosition = Vector3.zero;
        gameObject.transform.rotation = Quaternion.identity;
        gameObject.transform.localScale = Vector3.one;
        gameObject.SetActive(false);
    }

    private void OnTakeFromPool(GameObject gameObject)
    {
        gameObject.SetActive(_rootGameObject.activeSelf);
    }

    private void OnDestroyPoolObject(GameObject gameObject)
    {
        GameObject.Destroy(gameObject);
    }

    public GameObject Spawn()
    {
        return Pool.Get();
    }

    public void DeSpawn(GameObject gameObject)
    {
        Pool.Release(gameObject);
    }

    public void Dispose()
    {
        _rootGameObject = null;
        _pool.Clear();
    }
}

public enum PoolType
{
    Stack,
    LinkedList
}

public static class GameObjectPoolSOExtention
{
    public static GameObject TryDespawnOldAndSpawnNew(this GameObjectPoolSO pool, GameObject old, GameObject sourceNew, Transform parent)
    {
        if (old)
            pool.DeSpawn(old);
        return pool.Spawn(sourceNew, parent);
    }
}