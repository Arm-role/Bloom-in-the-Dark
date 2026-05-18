using System;

public interface ICycleController
{
    event Action OnCycleCompleted;
    void StartCycle();
    void StopCycle();
}
