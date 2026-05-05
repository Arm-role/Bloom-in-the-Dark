public class WaveRuntime
{
  private readonly WaveDefinition _data;
  private readonly EnemyCounter _enemyCounter;

  private readonly SpawnScheduler _singleScheduler;
  private readonly SpawnScheduler _normalScheduler;
  private readonly SpawnScheduler _endBurstScheduler;

  private float _time;
  private bool _singleSpawned;

  public bool IsFinished
  {
    get
    {
      if (_data.Type == WaveType.Single)
        return _singleSpawned && _enemyCounter.IsClear;

      return _time >= _data.Duration;
    }
  }

  public WaveRuntime(
    WaveDefinition data,
    EnemyCounter counter,
    IEntitySpawner spawner)
  {
    _data = data;
    _enemyCounter = counter;
    
    switch (_data.Type)
    {
      case WaveType.Single:
        _singleScheduler = new SpawnScheduler(
          _data.NormalSpawn,
          spawner);
        break;

      case WaveType.Normal:
      case WaveType.Burst:
        _normalScheduler = new SpawnScheduler(
          _data.NormalSpawn,
          spawner);
        _endBurstScheduler = new SpawnScheduler(
          _data.EndBurstSpawn,
          spawner);
        break;
    }
  }

  public void Tick(float dt)
  {
    if (IsFinished)
      return;

    _time += dt;

    switch (_data.Type)
    {
      case WaveType.Single:
        TickSingle();
        break;

      case WaveType.Normal:
        TickNormal(dt);
        break;

      case WaveType.Burst:
        TickBurst(dt);
        break;
    }
  }

  private void TickSingle()
  {
    if (_singleSpawned)
      return;

    if (_data.SpawnAtStart || _time >= _data.Duration * 0.5f)
    {
      _singleScheduler.Tick(float.MaxValue); // force spawn
      _singleSpawned = true;
    }
  }

  private void TickNormal(float dt)
  {
    if (_time < _data.Duration * _data.NextWaveStartRatio)
      _normalScheduler.Tick(dt);
    else
      _endBurstScheduler.Tick(dt);
  }

  private void TickBurst(float dt)
  {
    if (_time >= _data.Duration * _data.NextWaveStartRatio)
      _endBurstScheduler.Tick(dt);
  }
}