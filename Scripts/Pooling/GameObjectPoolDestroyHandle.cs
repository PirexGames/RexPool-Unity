using UnityEngine;
public class GameObjectPoolDestroyHandle : MonoBehaviour
{
    [SerializeField] private GameObjectPoolSO pool;

    private void Awake()
    {
        pool.SetUpDespawnParent(transform);
    }

    private void OnDestroy()
    {
        pool?.Dispose();
    }

    private void OnApplicationQuit()
    {
        OnDestroy();
    }
}

