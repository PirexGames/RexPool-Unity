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

        public static bool GetGameObject<T>(RexPoolObject prefab, out T typeObject, bool activeObject = false) where T : RexPoolObject
        {
            typeObject = null;
            var go = GetRexPoolObject(prefab, activeObject);
            if (go is T)
            {
                typeObject = go as T;
                return true;
            }
            return false;
        }

        public static GameObject GetGameObject(RexPoolObject prefab, bool activeObject = false) => GetRexPoolObject(prefab, activeObject).gameObject;

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