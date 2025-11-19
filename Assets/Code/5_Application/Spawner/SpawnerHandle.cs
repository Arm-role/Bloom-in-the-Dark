using System;
using System.Threading.Tasks;
using UnityEngine;

public class SpawnerHandle
{
    private readonly GameObjectSpawner _spawner;

    public event Action<GameObject> OnSpawnCompleted;
    public event Action<GameObject> OnDespawnCompleted;

    public SpawnerHandle(GameObjectSpawner spawner)
    {
        _spawner = spawner;
    }

    private async Task<GameObject> CoreSpawn(Func<Task<GameObject>> spawnAction)
    {
        var ob = await spawnAction();

        if (ob.TryGetComponent<IPoolable<GameObject>>(out var poolable))
        {
            poolable.OnSpawnFromPool(ob);
        }

        OnSpawnCompleted?.Invoke(ob);
        return ob;
    }
    public void Despawn(GameObject ob)
    {
        if (ob.TryGetComponent<IPoolable<GameObject>>(out var poolable))
        {
            poolable.OnReturnToPool(ob);
        }

        OnDespawnCompleted?.Invoke(ob);
        _spawner.Despawn(ob);
    }

    public async Task<GameObject> SpawnAsync(string name, Vector3 position)
    {
        return await CoreSpawn(() => _spawner.SpawnAsync(name, position));
    }

    public async Task<GameObject> SpawnAsync(int id, Vector3 position)
    {
        return await CoreSpawn(() => _spawner.SpawnAsync(id, position));
    }
}