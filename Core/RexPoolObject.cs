using System;
using System.Collections.Generic;
using UnityEngine;

namespace PirexGames.RexPool
{
    [DisallowMultipleComponent]
    public class RexPoolObject : MonoBehaviour
    {
        internal int prefabId;
        internal bool isOnPool;

        // Cache component references to avoid repeated GetComponent calls
        private Dictionary<Type, Component> _componentCache;

        /// <summary>
        /// Returns a cached component reference — zero GetComponent overhead after first call.
        /// </summary>
        public T GetCachedComponent<T>() where T : Component
        {
            _componentCache ??= new Dictionary<Type, Component>(4);
            var type = typeof(T);
            if (_componentCache.TryGetValue(type, out var cached) && cached != null)
                return (T)cached;
            var component = GetComponent<T>();
            if (component != null)
                _componentCache[type] = component;
            return component;
        }

        /// <summary>
        /// Auto-return to pool when disabled. Guard prevents double-push.
        /// </summary>
        protected virtual void OnDisable()
        {
            if (prefabId == 0 || isOnPool) return;
            isOnPool = true;
            RexPoolManager.PushToFreePool(prefabId, gameObject);
        }

        protected virtual void OnDestroy()
            => RexPoolManager.OnInstanceDestroyed(gameObject.GetInstanceID(), prefabId);

        /// <summary>
        /// Return this object to the pool. Triggers OnDisable which handles the push.
        /// </summary>
        public void ReturnToPool() => gameObject.SetActive(false);
    }
}
