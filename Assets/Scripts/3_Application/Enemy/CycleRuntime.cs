using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CycleRuntime
{
  public event Action OnBossKilled;

  private readonly WaveDefinition _wave;
  private readonly IEntitySpawner _spawner;
  private readonly IEnemyCounter _counter;

  private readonly int _day;
  private readonly float _hpMultiplier;
  private readonly float _damageMultiplier;

  private readonly List<EnemyEntry> _availablePool = new();

  private int _remainingPool;
  private float _spawnTimer;

  public bool CanEndCycle => _remainingPool <= 0 && _counter.IsClear;

  public CycleRuntime(
    WaveDefinition wave,
    IEntitySpawner spawner,
    IEnemyCounter counter,
    int day)
  {
    _wave = wave;
    _spawner = spawner;
    _counter = counter;
    _day = day;

    _remainingPool = Mathf.Max(1,
      Mathf.RoundToInt(EvaluateCurve(wave.poolCountByDay, day, 30f)));

    _hpMultiplier = Mathf.Max(0.01f,
      EvaluateCurve(wave.hpMultiplierByDay, day, 1f));

    _damageMultiplier = Mathf.Max(0.01f,
      EvaluateCurve(wave.damageMultiplierByDay, day, 1f));

    FilterPoolByDay();

    _spawnTimer = 0f;

    if (ShouldSpawnBoss())
      SpawnBoss();
  }

  private bool ShouldSpawnBoss()
  {
    return _wave.bossEnemy != null
        && _wave.bossDayInterval > 0
        && _day > 0
        && _day % _wave.bossDayInterval == 0;
  }

  private async void SpawnBoss()
  {
    var pattern = _wave.spawn;
    float radius = pattern != null ? pattern.MaxRadius : 8f;
    float angle = Random.Range(0f, Mathf.PI * 2f);
    Vector3 pos = new Vector3(
        Mathf.Cos(angle) * radius,
        Mathf.Sin(angle) * radius,
        0f);

    float bossHpMul = Mathf.Max(0.01f,
      EvaluateCurve(_wave.bossHpMultiplierByDay, _day, 1f));
    float bossDmgMul = Mathf.Max(0.01f,
      EvaluateCurve(_wave.bossDamageMultiplierByDay, _day, 1f));

    try
    {
      var entity = await _spawner.Spawn(_wave.bossEnemy.RuntimeTag.Hash, pos);

      if (entity is EnemyController boss)
      {
        boss.ApplyDayScaling(bossHpMul, bossDmgMul);

        if (GlobalTargetProvider.Instance != null)
          boss.AssignTarget(GlobalTargetProvider.Instance.Player);

        Action<GameObject> handler = null;
        handler = _ =>
        {
          boss.OnRequestDestruction -= handler;
          OnBossKilled?.Invoke();
        };
        boss.OnRequestDestruction += handler;
      }
    }
    catch (System.Exception ex)
    {
      Debug.LogError($"[CycleRuntime] Boss spawn failed: {ex.Message}");
    }
  }

  private void FilterPoolByDay()
  {
    _availablePool.Clear();
    if (_wave.spawn?.EnemyPool == null) return;

    foreach (var entry in _wave.spawn.EnemyPool)
    {
      if (entry == null || entry.enemy == null) continue;
      if (entry.minDay > _day) continue;
      _availablePool.Add(entry);
    }
  }

  public void Tick(float dt)
  {
    if (_remainingPool <= 0) return;
    if (_availablePool.Count == 0) return;

    _spawnTimer -= dt;
    if (_spawnTimer > 0f) return;

    SpawnBatch();
    ResetTimer();
  }

  private async void SpawnBatch()
  {
    var pattern = _wave.spawn;
    int batchSize = Random.Range(pattern.MinCount, pattern.MaxCount + 1);
    batchSize = Mathf.Min(batchSize, _remainingPool);
    _remainingPool -= batchSize;

    for (int i = 0; i < batchSize; i++)
    {
      var pick = _availablePool[Random.Range(0, _availablePool.Count)];

      float min = pattern.MinRadius;
      float max = pattern.MaxRadius;

      float radius = Mathf.Sqrt(Random.Range(min * min, max * max));
      float angle = Random.Range(0f, Mathf.PI * 2f);

      Vector3 pos = new Vector3(
          Mathf.Cos(angle) * radius,
          Mathf.Sin(angle) * radius,
          0f);

      try
      {
        var entity = await _spawner.Spawn(pick.enemy.RuntimeTag.Hash, pos);

        if (entity is EnemyController enemy)
        {
          enemy.ApplyDayScaling(_hpMultiplier, _damageMultiplier);

          if (GlobalTargetProvider.Instance != null)
            enemy.AssignTarget(GlobalTargetProvider.Instance.Player);
        }
      }
      catch (System.Exception ex)
      {
        Debug.LogError($"[CycleRuntime] Spawn failed: {ex.Message}");
      }
    }
  }

  private void ResetTimer()
  {
    var p = _wave.spawn;
    float min = Mathf.Max(0.01f, p.MinInterval);
    float max = Mathf.Max(min, p.MaxInterval);
    _spawnTimer = Random.Range(min, max);
  }

  private static float EvaluateCurve(AnimationCurve curve, float x, float defaultValue)
  {
    if (curve == null || curve.length == 0) return defaultValue;
    return curve.Evaluate(x);
  }
}
