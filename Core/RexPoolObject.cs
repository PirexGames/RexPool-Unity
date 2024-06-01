using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PirexGames.RexPool
{
    [DisallowMultipleComponent]
    public class RexPoolObject : MonoBehaviour
    {
        internal int prefabId;
        internal bool isOnPool;
        protected virtual void OnDisable() => isOnPool = true;
        protected virtual void OnDestroy() => this.gameObject.Release(prefabId);
        public void ReturnToPool()
        {
            isOnPool = true;
            gameObject.SetActive(false);
        }
    }

}
