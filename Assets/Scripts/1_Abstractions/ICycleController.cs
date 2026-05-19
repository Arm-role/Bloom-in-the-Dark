using System;

public interface ICycleController
{
    event Action OnCycleCompleted;
    event Action OnBossKilled;
    void StartCycle(int day);
    void StopCycle();
}
