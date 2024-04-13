using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PirexGames.RexPool
{
    [DisallowMultipleComponent]
    public class DelayReturnRPO : MonoBehaviour
    {
        private void OnEnable()
        {
            Invoke("DisableRPO", 3f);
        }

        private void DisableRPO()
        {
            gameObject.SetActive(false);
        }
    }
}
