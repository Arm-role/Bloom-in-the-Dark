using System;

public class EnemyCounter
{
    public int AliveCount { get; private set; }
    public bool IsClear => AliveCount <= 0;

    public event Action<int> OnChanged;

    public void OnEnemySpawned()
    {
        AliveCount++;
        OnChanged?.Invoke(AliveCount);
    }

    public void OnEnemyKilled()
    {
        AliveCount--;
        OnChanged?.Invoke(AliveCount);
    }
}