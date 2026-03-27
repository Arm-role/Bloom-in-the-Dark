using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AdressablePoolingService : IAdressablePoolService<GameObject>
{
    private readonly Dictionary<GameObject, AsyncObjectPool<GameObject>> _pool = new();
    private readonly Transform _poolParent;

    public AdressablePoolingService()
    {
        _poolParent = new GameObject("[AdressablePoolingService_Root]").transform;
        Object.DontDestroyOnLoad(_poolParent.gameObject);
    }

    public async Task<GameObject> AsyncGet(GameObject prefab)
    {
        if (!_pool.TryGetValue(prefab, out var pool))
        {
            var factory = new AdressbleGameObjectFactory(prefab, _poolParent);
            pool = new AsyncObjectPool<GameObject>(factory);
            _pool.Add(prefab, pool);
        }

        return await pool.GetAsync();
    }
    public void Return(GameObject prefab, GameObject instance)
    {
        if (_pool.TryGetValue(prefab, out var pool))
        {
            instance.SetActive(false);
            pool.Return(instance);
        }
        else
        {
            Debug.LogWarning($"Trying to return object to a non-existent pool: {prefab.name}. Destroying instead.");
            Object.Destroy(instance);
        }
    }
}
