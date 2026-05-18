using System;

public interface IEnemyCounter
{
    int AliveCount { get; }
    bool IsClear { get; }
    event Action<int> OnChanged;
}
