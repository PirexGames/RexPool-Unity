using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using PirexGames.RexPool;

namespace RexPoolExample
{
    public class RexPoolExample : MonoBehaviour
    {
        public RexPoolObject rexPoolObject;

        public Button btnSpawnWP;
        public Button btnPrepair;
        public Button btnLoadScene;

        public int amountPrepair;

        private void Awake()
        {
            btnSpawnWP.onClick.AddListener(SpawnWP);
            btnPrepair.onClick.AddListener(Prepair);
            btnLoadScene.onClick.AddListener(LoadScene);

        }

        private void SpawnWP()
        {
            for (int i = 0; i < 10; i++)
            {
                var wp = RexPoolManager.Take<Weapon>(rexPoolObject);
                wp.transform.position = Random.insideUnitCircle * 5;
                wp.Attack();
            }
        }

        private void Prepair()
        {
            RexPoolManager.Prepair(rexPoolObject, amountPrepair);
        }

        private void LoadScene()
        {
            SceneManager.LoadSceneAsync(0);
        }

    }
}
