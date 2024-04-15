using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PirexGames.RexPool
{
    [DisallowMultipleComponent]
    public class RexPoolObject : MonoBehaviour
    {
        internal int baseInstanceId;
        internal bool isOnPool;
        protected virtual void OnDisable() => isOnPool = true;
        protected virtual void OnDestroy() => this.Release();
        public void ReturnToPool()
        {
            isOnPool = true;
            gameObject.SetActive(false);
        }
    }

}
