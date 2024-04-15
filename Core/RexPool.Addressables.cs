#if REXPOOL_ADDRESSABLE
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement;
#endif
using System.Threading.Tasks;
using UnityEngine;

namespace PirexGames.RexPool
{
    public static partial class RexPool
    {

#if REXPOOL_ADDRESSABLE

        public static async Task Prepair(string addressableKey, int amount)
        {
            for (int i = 0; i < amount; i++)
                await Take(addressableKey, false);
        }

        public static void CleanUp(string addressableKey)
        {
            if (!_addressableCache.TryGetValue(addressableKey, out var go)) return;
            _addressableCache.Remove(addressableKey);
            CleanUp(go);
            GameObject.Destroy(go);
        }

        /// <summary>
        /// Get pool object by addressable key
        /// </summary>
        /// <param name="addressableKey">Key addressable of prefab</param>
        /// <param name="activeObject">Set active gameObject to value</param>
        public static async Task<GameObject> Take(string addressableKey, bool activeObject = false)
        {
            if (_addressableCache.TryGetValue(addressableKey, out var go))
            {
                // if have in addressable cache, get it
                return Take(go, activeObject);
            }
            // load new gameobject addressable
            var addressGO = await Addressables.LoadAssetAsync<GameObject>(addressableKey).Task;
            var rpo = addressGO.GetComponent<RexPoolObject>();
            if (!rpo)
            {
                Debug.LogError("Object must be RexPoolObject");
                return null;
            }
            _addressableCache.Add(addressableKey, rpo);

            return Take(rpo, activeObject);
        }

        public static async Task<T> Take<T>(string addressableKey, bool activeObject = false) where T : RexPoolObject
        {
            var go = await Take(addressableKey, activeObject);
            if (go is T)
                return go as T;
            Debug.Log("Cannot Parse Type T");
            return null;
        }
#endif

    }
}