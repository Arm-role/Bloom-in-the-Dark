using UnityEngine;

public class SpawnMock : MonoBehaviour
{
  public Transform player;
  public LayerMask playerMask;
  public LayerMask enemyMask;
  public LayerMask obstacleMask;

  [SerializeField] private ObjectKey enemyKey;
  [SerializeField] private ObjectKey dummyType;
  private EnemySpawner _spawner;

  public void Initialze(EnemySpawner spawner)
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

    //if (Input.GetKeyDown(KeyCode.K))
    //{
    //  Vector2 pointer = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //  SpawnDummy(pointer);
    //}
  }

  public async void SpawnEnemy(ObjectKey enemyKey, Vector3 position, float moveSpeed = 3f, int hp = 10)
  {
    _spawner.Spawn(enemyKey.RuntimeTag.Hash, position, moveSpeed, hp);
  }

  //public async void SpawnDummy(Vector3 position, int hp = 100)
  //{
  //  var go = await _spawnHandle.SpawnAsync(dummyType.ToString(), position);
  //  var ctrl = go.GetComponent<DummyController>();
  //  ctrl.Initialize(hp);
  //}
}
