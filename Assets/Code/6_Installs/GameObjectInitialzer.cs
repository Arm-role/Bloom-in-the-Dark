using UnityEngine;

public class GameObjectInitialzer
{
  private readonly TurnSystem _turnSystem;
  private readonly SpawnerHandle _spawnerHandle;
  private readonly WorldInteractionExecutor _executor;

  public GameObjectInitialzer(
    TurnSystem turnSystem,
    SpawnerHandle spawnerHandle,
    WorldInteractionExecutor executor)
  {
    _turnSystem = turnSystem;
    _spawnerHandle = spawnerHandle;

    _spawnerHandle.OnSpawnCompleted += Subscribe;
    _spawnerHandle.OnDespawnCompleted += UnSubscribe;
    _executor = executor;
  }

  private void Subscribe(GameObject obj)
  {
    if (obj.TryGetComponent<EnemyController>(out var c))
    {
      c.OnGetLootable += Execute;
    }
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
    if (obj.TryGetComponent<EnemyController>(out var c))
    {
      c.OnGetLootable -= Execute;
    }

    if (obj.TryGetComponent<IGrowthEntity>(out var growth))
    {
      _turnSystem.OnNextTurn -= growth.OnTurnPassed;
    }

    if (obj.TryGetComponent<IDestructible>(out var des))
    {
      des.OnRequestDestruction -= _spawnerHandle.Despawn;
    }
  }

  private async void Execute(WorldAction action)
  {
   await _executor.Execute(action);
  }
}