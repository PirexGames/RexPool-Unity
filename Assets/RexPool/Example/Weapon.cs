using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PirexGames.RexPool;

namespace RexPoolExample
{
    public class Weapon : RexPoolObject
    {

        private void OnEnable()
        {
            Invoke("ReturnPool", 3f);
        }

        private void ReturnPool()
        {
            ReturnToPool();
        }

        public void Attack()
        {
            Debug.Log("Attacking");
        }
    }
}
