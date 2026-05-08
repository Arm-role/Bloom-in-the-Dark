public class SingleWaveMode : IWaveMode
{
    private readonly SpawnScheduler _scheduler;
    private readonly IEnemyCounter _enemyCounter;
    private readonly float _duration;
    private readonly bool _spawnAtStart;
    private float _time;
    private bool _spawned;

    public bool IsFinished => _spawned && _enemyCounter.IsClear;

    public SingleWaveMode(WaveDefinition data, IEnemyCounter enemyCounter, IEntitySpawner spawner)
    {
        _scheduler      = new SpawnScheduler(data.NormalSpawn, spawner);
        _enemyCounter   = enemyCounter;
        _duration       = data.Duration;
        _spawnAtStart   = data.SpawnAtStart;
    }

    public void Tick(float dt)
    {
        if (_spawned) return;
        _time += dt;

        if (_spawnAtStart || _time >= _duration * 0.5f)
        {
            _scheduler.Tick(float.MaxValue);
            _spawned = true;
        }
    }
}
