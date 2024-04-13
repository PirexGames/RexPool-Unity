using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PirexGames.RexPool
{
    [DisallowMultipleComponent]
    public class RexPoolObject : MonoBehaviour
    {
        internal int _baseInstanceId;
        protected virtual void OnDestroy() => this.Release();
        public void ReturnToPool() => gameObject.SetActive(false);
    }

}
