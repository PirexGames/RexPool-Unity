using System;
using System.Collections.Generic;
using UnityEngine;

namespace PirexGames.RexPool
{
    public static partial class RexPool
    {
        private static Dictionary<int, List<RexPoolObject>> _pool = new();
        private static Dictionary<string, RexPoolObject> _addressableCache = new();

        public static void Prepair(RexPoolObject prefab, int amount)
        {
            for (int i = 0; i < amount; i++)
                Take(prefab, false);
        }

        public static void CleanUp(RexPoolObject prefab)
        {
            var prefabId = prefab.GetInstanceID();
            var pool = GetPool(prefabId);
            foreach (var item in pool)
            {
                try
                {
                    GameObject.Destroy(item.gameObject);
                }
                catch
                {
                    // ignore
                }
            }
            pool.Clear();
            _pool.Remove(prefabId);

        }

        /// <summary>
        /// This method return true false result get object from pool, result type gameObject will return from result value
        /// </summary>
        /// <param name="prefab">Prefab to pooling</param>
        /// <param name="result">Value type of Object</param>
        /// <param name="activeObject">Set active gameObject to value</param>
        public static T Take<T>(RexPoolObject prefab, bool activeObject = false) where T : RexPoolObject
        {
            var go = TakeRPO(prefab, activeObject);
            if (go is T)
                return go as T;
            Debug.Log("Cannot Parse Type T");
            return null;
        }

        /// <summary>
        /// Get GameObject from Pool
        /// </summary>
        /// <param name="prefab">Prefab to pooling</param>
        /// <param name="activeObject">Set active gameObject to value</param>
        public static GameObject Take(RexPoolObject prefab, bool activeObject = false) => TakeRPO(prefab, activeObject).gameObject;

        /// <summary>
        /// Get RexPoolObject from Pool
        /// </summary>
        /// <param name="prefab">Prefab to pooling</param>
        /// <param name="activeObject">Set active gameObject to value</param>
        public static RexPoolObject TakeRPO(RexPoolObject prefab, bool activeObject = false)
        {
            var prefabId = prefab.GetInstanceID();
            var pool = GetPool(prefabId);
            Debug.Log(pool.Count);
            for (int i = pool.Count - 1; i >= 0; i--)
            {
                if (pool[i].gameObject.activeSelf) continue;
                if (activeObject)
                    pool[i].gameObject.SetActive(true);
                return pool[i];
            }
            var go = GameObject.Instantiate(prefab);
            go._baseInstanceId = prefabId;
            pool.Add(go);
            return go;
        }

        /// <summary>
        /// Frees the object reference from the pool
        /// </summary>
        public static void Release(this RexPoolObject go)
        {
            var pool = GetPool(go._baseInstanceId);
            for (int i = pool.Count - 1; i >= 0; i--)
                if (pool[i].GetInstanceID().Equals(go.GetInstanceID())) pool.RemoveAt(i);
        }

        private static List<RexPoolObject> GetPool(int type)
        {
            if (_pool.TryGetValue(type, out var pool)) return pool;
            var newPool = new List<RexPoolObject>();
            _pool.TryAdd(type, newPool);
            return newPool;
        }
    }
}