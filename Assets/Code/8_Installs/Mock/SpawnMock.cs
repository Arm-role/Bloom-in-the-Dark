using System.Reflection;
using UnityEngine;

public class SpawnMock : MonoBehaviour
{
    public Transform player;
    public LayerMask playerMask;
    public LayerMask enemyMask;
    public LayerMask obstacleMask;

    private SpawnerHandle _spawnHandle;

    public void Initialze(SpawnerHandle spawner)
    {
        _spawnHandle = spawner;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            Vector2 pointer = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            SpawnEnemy(pointer);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            Vector2 pointer = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            SpawnDummy(pointer);
        }
    }

    public async void SpawnEnemy(Vector3 position, float moveSpeed = 3f, int hp = 10)
    {
        var go = await _spawnHandle.SpawnAsync("Enemy", position);
        var ctrl = go.GetComponent<EnemyController>();
        ctrl.Initialize(player, moveSpeed, hp);

        // sensor masks
        ctrl.Sensor.targetMask = playerMask;
        ctrl.Sensor.obstacleMask = obstacleMask; // adjust

        // movement masks
        ctrl.Steering.obstacleMask = obstacleMask;

        // add skills
        LayerMask targetMask = playerMask;
        ctrl.AddSkill(new MeleeSkill(range: 1.2f, damage: 3, cooldown: 1.2f, mask: targetMask));
        ctrl.AddSkill(new DashSkill(dashSpeed: 6f, duration: 1f, damage: 4, cooldown: 1f, 2, 4, mask: targetMask));
        //ctrl.AddSkill(new AOESlamSkill(radius: 1.6f, damage: 5, cooldown: 6f, mask: targetMask));
    }

    public async void SpawnDummy(Vector3 position, int hp = 100)
    {
        var go = await _spawnHandle.SpawnAsync("Dummy", position);
        var ctrl = go.GetComponent<DummyController>();
        ctrl.Initialize(hp);
    }
}