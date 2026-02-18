using UnityEngine;

public class GameObjectInitialzer
{
  private readonly TurnSystem _turnSystem;
  private readonly SpawnerHandle _spawnerHandle;

  public GameObjectInitialzer(
    TurnSystem turnSystem,
    SpawnerHandle spawnerHandle)
  {
    _turnSystem = turnSystem;
    _spawnerHandle = spawnerHandle;

    _spawnerHandle.OnSpawnCompleted += Subscribe;
    _spawnerHandle.OnDespawnCompleted += UnSubscribe;
  }

  private void Subscribe(GameObject obj)
  {
    if (obj.TryGetComponent<IGrowthEntity>(out var growth))
    {
      _turnSystem.OnNextTurn += growth.OnTurnPassed;
    }

    if (obj.TryGetComponent<IDestructible>(out var des))
    {
      des.OnRequestDestruction += _spawnerHandle.Despawn;
    }
  }

  private void UnSubscribe(GameObject obj)
  {
    if (obj.TryGetComponent<IGrowthEntity>(out var growth))
    {
      _turnSystem.OnNextTurn -= growth.OnTurnPassed;
    }

    if (obj.TryGetComponent<IDestructible>(out var des))
    {
      des.OnRequestDestruction -= _spawnerHandle.Despawn;
    }
  }
}