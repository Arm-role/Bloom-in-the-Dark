using System.Threading.Tasks;
using UnityEngine;
public class SpawnScheduler
{
  private readonly SpawnPattern _pattern;
  private readonly IEntitySpawner _spawner;

  private float _timer;

  public SpawnScheduler(
    SpawnPattern pattern,
    IEntitySpawner spawner)
  {
    _pattern = pattern;
    _spawner = spawner;
    ResetTimer();
  }

  public void Tick(float dt)
  {
    _timer -= dt;

    if (_timer > 0f)
      return;

    SpawnBatch();
    ResetTimer();
  }

  private async Task SpawnBatch()
  {
    int count = Random.Range(_pattern.MinCount, _pattern.MaxCount + 1);

    for (int i = 0; i < count; i++)
    {
      ObjectKey key = _pattern.EnemyPool[
        Random.Range(0, _pattern.EnemyPool.Length)];

      float min = _pattern.MinRadius;
      float max = _pattern.MaxRadius;

      float radius = Mathf.Sqrt(Random.Range(min * min, max * max));
      float angle = Random.Range(0f, Mathf.PI * 2f);

      Vector3 pos = new Vector3(
          Mathf.Cos(angle) * radius,
          Mathf.Sin(angle) * radius,
          0f
      );

      var entity = await _spawner.Spawn(key.RuntimeTag.Hash, pos);

      if (GlobalTargetProvider.Instance != null && entity != null && entity is EnemyController enemy)
        enemy.AssignTarget(GlobalTargetProvider.Instance.Player);
    }
  }

  private void ResetTimer()
  {
    float min = Mathf.Max(0.01f, _pattern.MinInterval);
    float max = Mathf.Max(min, _pattern.MaxInterval);

    _timer = Random.Range(min, max);
  }
}