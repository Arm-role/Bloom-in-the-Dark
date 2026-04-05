using System.Collections.Generic;

public class CycleRuntime
{
  private readonly CycleData _data;
  private readonly IEntitySpawner _spawner;
  private readonly EnemyCounter _enemyCounter;

  private readonly List<WaveRuntime> _activeWaves = new();
  private int _nextWaveIndex;

  private float _nextWaveTimer;

  public bool IsAllWavesStarted =>
    _nextWaveIndex >= _data.Waves.Length;

  public bool CanEndCycle
  {
    get
    {
      if (!IsAllWavesStarted)
        return false;

      if (_activeWaves.Count > 0)
        return false;

      return _enemyCounter.IsClear;
    }
  }

  public CycleRuntime(
    CycleData data,
    IEntitySpawner spawner,
    EnemyCounter enemyCounter)
  {
    _data = data;
    _spawner = spawner;
    _enemyCounter = enemyCounter;
  }

  public void Tick(float dt)
  {
    TryStartNextWave(dt);

    for (int i = _activeWaves.Count - 1; i >= 0; i--)
    {
      var wave = _activeWaves[i];
      wave.Tick(dt);

      if (wave.IsFinished)
        _activeWaves.RemoveAt(i);
    }
  }

  private void TryStartNextWave(float dt)
  {
    if (IsAllWavesStarted)
      return;

    _nextWaveTimer -= dt;
    if (_nextWaveTimer > 0f)
      return;

    var waveData = _data.Waves[_nextWaveIndex];
    var runtime = new WaveRuntime(waveData, _enemyCounter, _spawner);
    
    _activeWaves.Add(runtime);
    _nextWaveIndex++;

    _nextWaveTimer = waveData.Duration * waveData.NextWaveStartRatio;
  }
}