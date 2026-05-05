using System.Threading.Tasks;
using UnityEngine;

public class EntitySpawner : MonoBehaviour, IEntitySpawner
{
  public LayerMask playerMask;
  public LayerMask obstacleMask;

  private SpawnerHandle _spawnHandle;
  public EnemyCounter EnemyCounter = new EnemyCounter();

  public void Initialze(SpawnerHandle spawner)
  {
    _spawnHandle = spawner;

    _spawnHandle.OnSpawnCompleted += OnSpawn;
    _spawnHandle.OnDespawnCompleted += OnDespawn;
  }

  public async Task<EntityController> Spawn(
    int id,
    Vector3 position)
  {
    var go = await _spawnHandle.SpawnAsync(id, position);

    if (go == null)
    {
      Debug.LogError($"Spawn failed for id {id}");
      return null;
    }

    var ctrl = go.GetComponent<EntityController>();

    if (ctrl is EnemyController enemy) return enemy;

    else if (ctrl is DummyController dummy)
    {
      dummy.Initialize();
      return dummy;
    }

    return null;
  }

  private void OnSpawn(GameObject obj)
  {
    if (!obj.TryGetComponent<EnemyController>(out _)) return;
    EnemyCounter.OnEnemySpawned();
  }

  private void OnDespawn(GameObject obj)
  {
    if (!obj.TryGetComponent<EnemyController>(out _)) return;
    EnemyCounter.OnEnemyKilled();
  }
}