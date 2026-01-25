using System.Collections.Generic;
using UnityEngine;

public class CycleRuntime
{
    private readonly CycleData _data;
    private readonly IEnemySpawner _spawner;
    private readonly EnemyCounter _enemyCounter;

    private readonly List<WaveRuntime> _activeWaves = new();
    private int _nextWaveIndex;
    
    private float _nextWaveTimer;
    public bool IsAllWavesStarted =>
        _nextWaveIndex >= _data.Waves.Length;

    public bool CanEndCycle =>
        IsAllWavesStarted && _enemyCounter.IsClear;

    public CycleRuntime(
        CycleData data,
        IEnemySpawner spawner,
        EnemyCounter enemyCounter)
    {
        _data = data;
        _spawner = spawner;
        _enemyCounter = enemyCounter;
    }

    public void Tick(float dt)
    {
        Debug.Log($"TryStartNextWave : {_nextWaveIndex} >= {_data.Waves.Length}");
        
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
        if (_nextWaveIndex >= _data.Waves.Length)
            return;

        _nextWaveTimer -= dt;
        if (_nextWaveTimer > 0f)
            return;

        var waveData = _data.Waves[_nextWaveIndex];
        var runtime = new WaveRuntime(waveData, _spawner);

        _activeWaves.Add(runtime);
        _nextWaveIndex++;
        
        _nextWaveTimer = waveData.Duration * waveData.NextWaveStartRatio;
    }
}