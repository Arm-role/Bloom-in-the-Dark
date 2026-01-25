using UnityEngine;

public class SpawnScheduler
{
    private readonly SpawnPattern _pattern;
    private readonly IEnemySpawner _spawner;
    private float _timer;

    public SpawnScheduler(SpawnPattern pattern, IEnemySpawner spawner)
    {
        _pattern = pattern;
        _spawner = spawner;
        ResetTimer();
    }

    public void Tick(float dt)
    {
        _timer -= dt;
        Debug.Log($"_pattern : {_pattern.MinCount } : {_pattern.MaxCount + 1}");
        Debug.Log($"{_timer} > 0");
        
        if (_timer > 0f)
            return;

        SpawnBatch();
        ResetTimer();
    }

    private void SpawnBatch()
    {
        int count = Random.Range(_pattern.MinCount, _pattern.MaxCount + 1);

        Debug.Log($"count : {count}");

        for (int i = 0; i < count; i++)
        {
            EnemyType type = _pattern.EnemyPool[
                Random.Range(0, _pattern.EnemyPool.Length)];

            Vector3 pos = Random.insideUnitSphere * 10f;
            pos.y = 0f;

            Debug.Log($"Spawn : {type} : {pos}");
            _spawner.Spawn(type, pos);
        }
    }

    private void ResetTimer()
    {
        float min = Mathf.Max(0.01f, _pattern.MinInterval);
        float max = Mathf.Max(min, _pattern.MaxInterval);

        _timer = Random.Range(min, max);
    }
}