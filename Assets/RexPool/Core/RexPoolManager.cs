using System;
using System.Collections.Generic;
using UnityEngine;

namespace PirexGames.RexPool
{
    public static partial class RexPoolManager
    {
        private static Dictionary<int, List<GameObject>> _pool = new();
        private static Dictionary<string, GameObject> _addressableCache = new();

        public static void Prepair(GameObject prefab, int amount) => PrepairRPO(prefab, amount);

        public static void CleanUp(GameObject prefab)
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
        public static T Take<T>(GameObject prefab, bool activeObject = true) where T : MonoBehaviour
        {
            var go = TakeRPO(prefab, activeObject);
            var result = go.GetComponent<T>();
            if (result)
                return result;
            Debug.Log("Cannot Parse Type T");
            return default(T);
        }

        /// <summary>
        /// Get GameObject from Pool
        /// </summary>
        /// <param name="prefab">Prefab to pooling</param>
        /// <param name="activeObject">Set active gameObject to value</param>
        public static GameObject Take(GameObject prefab, bool activeObject = true) => TakeRPO(prefab, activeObject).gameObject;

        /// <summary>
        /// Get GameObject from Pool
        /// </summary>
        /// <param name="prefab">Prefab to pooling</param>
        /// <param name="activeObject">Set active gameObject to value</param>
        public static GameObject TakeRPO(GameObject prefab, bool activeObject = true)
        {
            var prefabId = prefab.GetInstanceID();
            var pool = GetPool(prefabId);
            for (int i = pool.Count - 1; i >= 0; i--)
            {
                var rexPoolObj = pool[i].GetComponent<RexPoolObject>();
                if (!rexPoolObj.isOnPool) continue;
                if (activeObject)
                    pool[i].gameObject.SetActive(true);
                rexPoolObj.isOnPool = false;
                return pool[i];
            }
            var go = GameObject.Instantiate(prefab);
            go.SetActive(activeObject);
            var poolObj = go.GetComponent<RexPoolObject>();
            if (!poolObj)
                poolObj = go.AddComponent<RexPoolObject>();
            poolObj.isOnPool = false;
            poolObj.prefabId = prefabId;
            pool.Add(go);
            return go;
        }

        private static void PrepairRPO(GameObject prefab, int amount)
        {
            var prefabId = prefab.GetInstanceID();
            var pool = GetPool(prefabId);
            for (int i = amount - 1; i >= 0; i--)
            {
                var go = GameObject.Instantiate(prefab);
                go.SetActive(false);
                var poolObj = go.GetComponent<RexPoolObject>();
                if (!poolObj)
                    poolObj = go.AddComponent<RexPoolObject>();
                poolObj.prefabId = prefabId;
                poolObj.isOnPool = true;
                pool.Add(go);
            }
        }

        /// <summary>
        /// Frees the object reference from the pool
        /// </summary>
        public static void Release(this GameObject go, int prefabId)
        {
            var pool = GetPool(prefabId);
            for (int i = pool.Count - 1; i >= 0; i--)
                if (pool[i].GetInstanceID().Equals(go.GetInstanceID())) pool.RemoveAt(i);
        }

        private static List<GameObject> GetPool(int type)
        {
            if (_pool.TryGetValue(type, out var pool)) return pool;
            var newPool = new List<GameObject>();
            _pool.TryAdd(type, newPool);
            return newPool;
        }
    }
}