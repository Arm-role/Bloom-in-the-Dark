using UnityEngine;

public class WaveRuntime
{
    private readonly WaveDefinition _data;
    private readonly SpawnScheduler _normalScheduler;
    private readonly SpawnScheduler _endBurstScheduler;

    private float _time;

    public bool IsFinished => _time >= _data.Duration;

    public WaveRuntime(
        WaveDefinition data,
        IEnemySpawner spawner)
    {
        _data = data;

        _normalScheduler = new SpawnScheduler(
            _data.NormalSpawn,
            spawner);

        _endBurstScheduler = new SpawnScheduler(
            _data.EndBurstSpawn,
            spawner);
    }

    public void Tick(float dt)
    {
        if (IsFinished)
            return;

        _time += dt;
        Debug.Log($"{_time} >= {_data.Duration}");

        if (_time < _data.EndBurstStartTime)
        {
            Debug.Log("Wave Tick");
            _normalScheduler.Tick(dt);
        }
        else
        {
            _endBurstScheduler.Tick(dt);
        }
    }
}