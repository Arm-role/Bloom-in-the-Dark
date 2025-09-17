using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class SpawnerHandle 
{
    private readonly GameObjectSpawner _spawner;

    public event Func<string, Vector2, Task<GameObject>> OnSpawnRequested;
    public event Action<GameObject> OnDespawnRequest;

    public event Action<Task<GameObject>> OnSpawnCompleted;

    public SpawnerHandle(GameObjectSpawner spawner)
    {
        _spawner = spawner;

        OnSpawnRequested += RequestSpawn;
        OnDespawnRequest += RequestDespawn;
    }

    private Task<GameObject> RequestSpawn(string gameObjectName, Vector2 position)
    {
        var ob = _spawner.SpawnOB(gameObjectName, position);
        OnSpawnCompleted?.Invoke(ob);

        return ob;
    }
    private void RequestDespawn(GameObject ob)
    {
        _spawner?.DespawnOB(ob);
    }
}
