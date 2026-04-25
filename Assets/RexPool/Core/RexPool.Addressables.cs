#if REXPOOL_ADDRESSABLE
using UnityEngine.AddressableAssets;
#endif
using System.Threading.Tasks;
using UnityEngine;

namespace PirexGames.RexPool
{
    public static partial class RexPoolManager
    {

#if REXPOOL_ADDRESSABLE

        /// <summary>Pre-warm pool with objects loaded from Addressables.</summary>
        public static async Task Prepair(string addressableKey, int amount)
        {
            if (!_addressableCache.TryGetValue(addressableKey, out var prefab))
            {
                prefab = await Addressables.LoadAssetAsync<GameObject>(addressableKey).Task;
                if (prefab == null)
                {
                    Debug.LogError($"[RexPool] Addressable '{addressableKey}' not found.");
                    return;
                }
                _addressableCache[addressableKey] = prefab;
            }
            PrepairRPO(prefab, amount);
        }

        /// <summary>Destroy all pooled instances loaded from the given addressable key.</summary>
        public static void CleanUp(string addressableKey)
        {
            if (!_addressableCache.TryGetValue(addressableKey, out var prefab)) return;
            _addressableCache.Remove(addressableKey);
            CleanUp(prefab);
        }

        /// <summary>Take a GameObject from the pool by addressable key.</summary>
        public static async Task<GameObject> Take(string addressableKey, bool activeObject = true)
        {
            if (!_addressableCache.TryGetValue(addressableKey, out var prefab))
            {
                prefab = await Addressables.LoadAssetAsync<GameObject>(addressableKey).Task;
                if (prefab == null)
                {
                    Debug.LogError($"[RexPool] Addressable '{addressableKey}' not found.");
                    return null;
                }
                _addressableCache[addressableKey] = prefab;
            }
            return Take(prefab, activeObject);
        }

        /// <summary>Take a typed component from the pool by addressable key.</summary>
        public static async Task<T> Take<T>(string addressableKey, bool activeObject = true)
            where T : MonoBehaviour
        {
            var go = await Take(addressableKey, activeObject);
            if (go == null) return default;
            var result = go.GetComponent<T>();
            if (result) return result;
            Debug.LogWarning($"[RexPool] Component {typeof(T).Name} not found on '{go.name}'");
            return default;
        }

#endif

    }
}