public class NormalWaveMode : IWaveMode
{
    private readonly SpawnScheduler _normalScheduler;
    private readonly SpawnScheduler _endBurstScheduler;
    private readonly float _duration;
    private readonly float _transitionRatio;
    private float _time;

    public bool IsFinished => _time >= _duration;

    public NormalWaveMode(WaveDefinition data, IEntitySpawner spawner)
    {
        _normalScheduler    = new SpawnScheduler(data.NormalSpawn,    spawner);
        _endBurstScheduler  = new SpawnScheduler(data.EndBurstSpawn,  spawner);
        _duration           = data.Duration;
        _transitionRatio    = data.TransitionRatio;
    }

    public void Tick(float dt)
    {
        if (IsFinished) return;
        _time += dt;

        if (_time < _duration * _transitionRatio)
            _normalScheduler.Tick(dt);
        else
            _endBurstScheduler.Tick(dt);
    }
}
