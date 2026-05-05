using UnityEngine;

public class SpawnMock : MonoBehaviour
{
  public FlowFieldTarget Target;
  public LayerMask playerMask;
  public LayerMask enemyMask;
  public LayerMask obstacleMask;

  [SerializeField] private ObjectKey enemyKey;
  [SerializeField] private ObjectKey dummyKey;
  private EntitySpawner _spawner;

  public void Initialze(EntitySpawner spawner)
  {
    _spawner = spawner;
  }

  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.M))
    {
      Vector2 pointer = Camera.main.ScreenToWorldPoint(Input.mousePosition);
      SpawnEnemy(enemyKey, pointer);
    }

    if (Input.GetKeyDown(KeyCode.N))
    {
      Vector2 pointer = Camera.main.ScreenToWorldPoint(Input.mousePosition);
      SpawnEnemy(dummyKey, pointer);
    }
  }

  public async void SpawnEnemy(ObjectKey enemyKey, Vector3 position)
  {
    var entity = await _spawner.Spawn(enemyKey.RuntimeTag.Hash, position);

    if (entity != null && entity is EnemyController enemy)
      enemy.AssignTarget(Target.transform);
  }
}
