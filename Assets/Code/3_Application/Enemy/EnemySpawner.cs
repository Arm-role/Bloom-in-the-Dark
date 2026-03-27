using UnityEngine;

public class EnemySpawner : MonoBehaviour, IEnemySpawner
{
  public Transform player;
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

  public async void Spawn(int id, Vector3 position, float moveSpeed = 3f, int hp = 10)
  {
    var go = await _spawnHandle.SpawnAsync(id, position);
    var ctrl = go.GetComponent<EnemyController>();
    ctrl.Initialize();
    ctrl.Setup(player, moveSpeed, hp);

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
            damage: 4,
            cooldown: 1f,
            2,
            4,
            mask: targetMask
        ));
        break;
    }

    ctrl.AddSkill(new MeleeSkill(
        range: 1.2f,
        damage: 3,
        cooldown: 1.2f,
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