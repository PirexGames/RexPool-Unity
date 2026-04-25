using System.Collections.Generic;
using UnityEngine;

namespace PirexGames.RexPool
{
    public static partial class RexPoolManager
    {
        // Available objects ready to reuse — O(1) push/pop
        private static readonly Dictionary<int, Stack<GameObject>> _freePool = new();

        // All instance IDs per prefab (free + in-use), used by CleanUp
        private static readonly Dictionary<int, List<int>> _allInstances = new();

        // Fast lookup: goInstanceID → RexPoolObject — eliminates GetComponent in hot path
        private static readonly Dictionary<int, RexPoolObject> _rpoCache = new();

        private static readonly Dictionary<string, GameObject> _addressableCache = new();

        // ─── Public API ──────────────────────────────────────────────────────────────

        /// <summary>Pre-warm the pool with a number of inactive instances.</summary>
        public static void Prepair(GameObject prefab, int amount) => PrepairRPO(prefab, amount);

        /// <summary>Destroy all pooled instances of this prefab and clear tracking data.</summary>
        public static void CleanUp(GameObject prefab)
        {
            var prefabId = prefab.GetInstanceID();

            if (_allInstances.TryGetValue(prefabId, out var ids))
            {
                foreach (var goId in ids)
                {
                    if (_rpoCache.TryGetValue(goId, out var rpo) && rpo != null)
                        Object.Destroy(rpo.gameObject);
                    _rpoCache.Remove(goId);
                }
                ids.Clear();
                _allInstances.Remove(prefabId);
            }

            if (_freePool.TryGetValue(prefabId, out var stack))
            {
                stack.Clear();
                _freePool.Remove(prefabId);
            }
        }

        /// <summary>
        /// Take a typed component from the pool.
        /// Uses cached component lookup — zero GetComponent overhead after first call.
        /// </summary>
        public static T Take<T>(GameObject prefab, bool activeObject = true) where T : MonoBehaviour
        {
            var go = TakeRPO(prefab, activeObject);
            if (_rpoCache.TryGetValue(go.GetInstanceID(), out var rpo))
            {
                var cached = rpo.GetCachedComponent<T>();
                if (cached) return cached;
            }
            var result = go.GetComponent<T>();
            if (result) return result;
            Debug.LogWarning($"[RexPool] Component {typeof(T).Name} not found on '{go.name}'");
            return default;
        }

        /// <summary>Take a GameObject from the pool.</summary>
        public static GameObject Take(GameObject prefab, bool activeObject = true)
            => TakeRPO(prefab, activeObject);

        /// <summary>
        /// Core take — O(1) amortized. Null-checks destroyed entries lazily.
        /// </summary>
        public static GameObject TakeRPO(GameObject prefab, bool activeObject = true)
        {
            var prefabId = prefab.GetInstanceID();

            if (_freePool.TryGetValue(prefabId, out var stack))
            {
                while (stack.Count > 0)
                {
                    var pooled = stack.Pop();
                    if (pooled == null) continue; // Destroyed externally — skip stale ref

                    if (_rpoCache.TryGetValue(pooled.GetInstanceID(), out var rpo))
                        rpo.isOnPool = false;

                    if (activeObject)
                        pooled.SetActive(true);

                    return pooled;
                }
            }

            return CreateInstance(prefab, prefabId, activeObject);
        }

        /// <summary>
        /// Permanently removes a specific instance from pool tracking.
        /// Called automatically from RexPoolObject.OnDestroy.
        /// </summary>
        public static void Release(this GameObject go, int prefabId)
        {
            var goId = go.GetInstanceID();
            _rpoCache.Remove(goId);
            if (_allInstances.TryGetValue(prefabId, out var ids))
                ids.Remove(goId);
        }

        // ─── Internal ────────────────────────────────────────────────────────────────

        /// <summary>
        /// Called from RexPoolObject.OnDisable. Pushes the object back to free stack.
        /// </summary>
        internal static void PushToFreePool(int prefabId, GameObject go)
        {
            if (!_freePool.TryGetValue(prefabId, out var stack))
            {
                stack = new Stack<GameObject>();
                _freePool[prefabId] = stack;
            }
            stack.Push(go);
        }

        /// <summary>
        /// Called from RexPoolObject.OnDestroy. Cleans up cache entries.
        /// </summary>
        internal static void OnInstanceDestroyed(int goInstanceId, int prefabId)
        {
            _rpoCache.Remove(goInstanceId);
            if (_allInstances.TryGetValue(prefabId, out var ids))
                ids.Remove(goInstanceId); // O(n) — acceptable; OnDestroy is rare
        }

        // ─── Private ─────────────────────────────────────────────────────────────────

        private static GameObject CreateInstance(GameObject prefab, int prefabId, bool activeObject)
        {
            var go = Object.Instantiate(prefab);
            go.SetActive(activeObject);

            var poolObj = go.GetComponent<RexPoolObject>() ?? go.AddComponent<RexPoolObject>();
            poolObj.isOnPool = false;
            poolObj.prefabId = prefabId;

            var goId = go.GetInstanceID();
            _rpoCache[goId] = poolObj;

            if (!_allInstances.TryGetValue(prefabId, out var ids))
            {
                ids = new List<int>();
                _allInstances[prefabId] = ids;
            }
            ids.Add(goId);

            return go;
        }

        private static void PrepairRPO(GameObject prefab, int amount)
        {
            var prefabId = prefab.GetInstanceID();

            if (!_freePool.TryGetValue(prefabId, out var stack))
            {
                stack = new Stack<GameObject>(amount);
                _freePool[prefabId] = stack;
            }

            if (!_allInstances.TryGetValue(prefabId, out var ids))
            {
                ids = new List<int>(amount);
                _allInstances[prefabId] = ids;
            }

            for (int i = 0; i < amount; i++)
            {
                var go = Object.Instantiate(prefab);
                var poolObj = go.GetComponent<RexPoolObject>() ?? go.AddComponent<RexPoolObject>();
                poolObj.prefabId = prefabId;

                // Set isOnPool = true BEFORE SetActive(false) to prevent OnDisable double-push
                poolObj.isOnPool = true;

                var goId = go.GetInstanceID();
                _rpoCache[goId] = poolObj;
                ids.Add(goId);

                go.SetActive(false); // Triggers OnDisable — guard (isOnPool=true) prevents re-push
                stack.Push(go);
            }
        }
    }
}