using UnityEngine;

public class SpawnMock : MonoBehaviour
{
  public Transform player;
  public LayerMask playerMask;
  public LayerMask enemyMask;
  public LayerMask obstacleMask;

  [SerializeField] private ObjectKey enemyKey;
  [SerializeField] private ObjectKey dummyKey;
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

    if (Input.GetKeyDown(KeyCode.N))
    {
      Vector2 pointer = Camera.main.ScreenToWorldPoint(Input.mousePosition);
      SpawnEnemy(dummyKey, pointer);
    }
  }

  public async void SpawnEnemy(ObjectKey enemyKey, Vector3 position, float moveSpeed = 3f, int hp = 10)
  {
    _spawner.Spawn(enemyKey.RuntimeTag.Hash, position, moveSpeed, hp);
  }
}
