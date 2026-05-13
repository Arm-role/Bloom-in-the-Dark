public class WaveRuntime
{
    private readonly IWaveMode _mode;

    public bool IsFinished => _mode.IsFinished;

    public WaveRuntime(WaveDefinition data, IEnemyCounter counter, IEntitySpawner spawner)
    {
        _mode = WaveModeFactory.Create(data, counter, spawner);
    }

    public void Tick(float dt)
    {
        if (IsFinished) return;
        _mode.Tick(dt);
    }
}
