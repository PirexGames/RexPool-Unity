using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

#if REXPOOL_ADDRESSABLE
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement;
#endif

namespace PirexGames.RexPool
{
    public static class RexPool
    {
        private static Dictionary<int, List<RexPoolObject>> _pool = new();
        private static Dictionary<string, RexPoolObject> _addressableCache = new();

#if REXPOOL_ADDRESSABLE
        /// <summary>
        /// Get pool object by addressable key
        /// </summary>
        /// <param name="addressableKey">Key addressable of prefab</param>
        /// <param name="activeObject">Set active gameObject to value</param>
        public static async Task<GameObject> GetGameObject(string addressableKey, bool activeObject = false)
        {
            if (_addressableCache.TryGetValue(addressableKey, out var go))
            {
                return GetGameObject(go, activeObject);
            }
            var addressGO = await Addressables.LoadAssetAsync<GameObject>(addressableKey).Task;
            var rpo = addressGO.GetComponent<RexPoolObject>();
            if (!rpo)
            {
                Debug.LogError("Object must be RexPoolObject");
                return null;
            }
            _addressableCache.Add(addressableKey, rpo);

            return GetGameObject(rpo, activeObject);
        }
#endif

        /// <summary>
        /// This method return true false result get object from pool, result type gameObject will return from result value
        /// </summary>
        /// <param name="prefab">Prefab to pooling</param>
        /// <param name="result">Value type of Object</param>
        /// <param name="activeObject">Set active gameObject to value</param>
        public static bool GetGameObject<T>(RexPoolObject prefab, out T result, bool activeObject = false) where T : RexPoolObject
        {
            result = null;
            var go = GetRexPoolObject(prefab, activeObject);
            if (go is T)
            {
                result = go as T;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Get GameObject from Pool
        /// </summary>
        /// <param name="prefab">Prefab to pooling</param>
        /// <param name="activeObject">Set active gameObject to value</param>
        public static GameObject GetGameObject(RexPoolObject prefab, bool activeObject = false) => GetRexPoolObject(prefab, activeObject).gameObject;

        /// <summary>
        /// Get RexPoolObject from Pool
        /// </summary>
        /// <param name="prefab">Prefab to pooling</param>
        /// <param name="activeObject">Set active gameObject to value</param>
        public static RexPoolObject GetRexPoolObject(RexPoolObject prefab, bool activeObject = false)
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