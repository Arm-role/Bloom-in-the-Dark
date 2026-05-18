public static class WaveModeFactory
{
    public static IWaveMode Create(WaveDefinition data, IEnemyCounter counter, IEntitySpawner spawner)
    {
        return data.Type switch
        {
            WaveType.Normal => new NormalWaveMode(data, spawner),
            WaveType.Burst  => new BurstWaveMode(data, spawner),
            WaveType.Single => new SingleWaveMode(data, counter, spawner),
            _ => throw new System.ArgumentOutOfRangeException(nameof(data.Type), data.Type, "Unknown WaveType")
        };
    }
}
