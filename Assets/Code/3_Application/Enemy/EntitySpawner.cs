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
    Vector3 position, 
    float moveSpeed = 3f,
    int hp = 10)
  {
    var go = await _spawnHandle.SpawnAsync(id, position);

    if (go == null)
    {
      Debug.LogError($"Spawn failed for id {id}");
      return null;
    }

    var ctrl = go.GetComponent<EntityController>();

    if (ctrl is EnemyController enemy)
    {
      InitializeEnemy(moveSpeed, hp, enemy);
      return enemy;
    }
    else if (ctrl is DummyController dummy)
    {
      dummy.Initialize(hp);
      return dummy;
    }

    return null;
  }

  private void InitializeEnemy(float moveSpeed, int hp, EnemyController ctrl)
  {
    ctrl.Initialize();
    ctrl.Setup(moveSpeed, hp);

    // sensor masks
    ctrl.Sensor.targetMask = playerMask;
    ctrl.Sensor.obstacleMask = obstacleMask;

    // movement masks
    ctrl.Steering.obstacleMask = obstacleMask;

    // add skills
    LayerMask targetMask = playerMask;

    switch (ctrl.Type)
    {
      case EnemyType.Helmet:
        ctrl.AddSkill(new DashSkill(
            dashSpeed: 6f,
            duration: 1f,
            damage: 10,
            cooldown: 2f,
            2,
            4,
            mask: targetMask
        ));
        break;
    }

    ctrl.AddSkill(new MeleeSkill(
        range: 1.2f,
        damage: 3,
        cooldown: 1f,
        mask: targetMask
    ));
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