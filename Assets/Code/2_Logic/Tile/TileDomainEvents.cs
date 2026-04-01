using System;

public static class TileDomainEvents
{
    public static event Action OnTileScanCompleted;
    public static event Action OnObstacleScanCompleted;

    public static void TileScanCompleted() => OnTileScanCompleted?.Invoke();
    public static void ObstacleScanCompleted() => OnObstacleScanCompleted?.Invoke();
}