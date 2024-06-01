using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PirexGames.RexPool;

namespace RexPoolExample
{
    public class Weapon : MonoBehaviour
    {

        private void OnEnable()
        {
            Invoke("ReturnPool", 3f);
        }

        private void ReturnPool()
        {
            this.gameObject.SetActive(false);
        }

        public void Attack()
        {
            Debug.Log("Attacking");
        }
    }
}
