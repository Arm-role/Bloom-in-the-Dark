using System;
using System.Threading.Tasks;
using UnityEngine;

public class SpawnerHandle
{
  private readonly ISpawner _spawner;

  public event Action<GameObject> OnSpawnCompleted;
  public event Action<GameObject> OnDespawnCompleted;

  public SpawnerHandle(ISpawner spawner)
  {
    _spawner = spawner;
  }

  private async Task<GameObject> CoreSpawn(Func<Task<GameObject>> spawnAction)
  {
    var ob = await spawnAction();
    return RegisterPool(ob);
  }

  private GameObject RegisterPool(GameObject ob)
  {
    if (ob.TryGetComponent<IPoolable<GameObject>>(out var poolable))
    {
      poolable.OnSpawnFromPool(ob);
      poolable.IsAlive = true;
    }

    OnSpawnCompleted?.Invoke(ob);
    return ob;
  }

  public void Despawn(GameObject ob)
  {
    if (ob.TryGetComponent<IPoolable<GameObject>>(out var poolable))
    {
      poolable.OnReturnToPool(ob);
      poolable.IsAlive = false;
    }

    OnDespawnCompleted?.Invoke(ob);
    _spawner.Despawn(ob);
  }

  public void Register(GameObject ob)
  {
    RegisterPool(ob);
  }

  public async Task<GameObject> SpawnAsync(int id, Vector3 position)
  {
    return await CoreSpawn(() => _spawner.SpawnAsync(id, position));
  }

  public async Task<GameObject> SpawnAsync(int id, Vector3 position, Vector2 direction)
  {
    return await CoreSpawn(() => _spawner.SpawnAsync(id, position, direction));
  }
}