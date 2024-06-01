#if REXPOOL_ADDRESSABLE
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement;
#endif
using System.Threading.Tasks;
using UnityEngine;

namespace PirexGames.RexPool
{
    public static partial class RexPoolManager
    {

#if REXPOOL_ADDRESSABLE

        public static async Task Prepair(string addressableKey, int amount)
        {
            GameObject goAddressable;
            if (_addressableCache.TryGetValue(addressableKey, out var go))
                goAddressable = go;
            else
            {
                var addressGO = await Addressables.LoadAssetAsync<GameObject>(addressableKey).Task;
                goAddressable = addressGO.GetComponent<GameObject>();
            }
            if (goAddressable)
            {
                _addressableCache.Add(addressableKey, goAddressable);
                PrepairRPO(goAddressable, amount);
            }
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
        public static async Task<GameObject> Take(string addressableKey, bool activeObject = true)
        {
            if (_addressableCache.TryGetValue(addressableKey, out var go))
            {
                // if have in addressable cache, get it
                return Take(go, activeObject);
            }
            // load new gameobject addressable
            var addressGO = await Addressables.LoadAssetAsync<GameObject>(addressableKey).Task;
            var rpo = addressGO.GetComponent<GameObject>();
            if (!rpo)
            {
                Debug.LogError("Object must be RexPoolObject");
                return null;
            }
            _addressableCache.Add(addressableKey, rpo);

            return Take(rpo, activeObject);
        }

        public static async Task<T> Take<T>(string addressableKey, bool activeObject = true) where T : MonoBehaviour
        {
            var go = await Take(addressableKey, activeObject);
            var result = go.GetComponent<T>();
            if (result)
                return result;
            Debug.Log("Cannot Parse Type T");
            return default(T);
        }

#endif

    }
}