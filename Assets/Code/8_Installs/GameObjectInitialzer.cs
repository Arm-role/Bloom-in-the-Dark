using UnityEngine;

public class GameObjectInitialzer
{
    private readonly TurnSystem _turnSystem;
    private readonly SpawnerHandle _spawnerHandle;
    private readonly WorldTileManager _worldTileManager;
    private readonly WorldInteractionExecutor _executor;

    public GameObjectInitialzer(
        TurnSystem turnSystem,
        SpawnerHandle spawnerHandle,
        WorldTileManager worldTileManager,
        WorldInteractionExecutor executor)
    {
        _turnSystem = turnSystem;
        _spawnerHandle = spawnerHandle;
        _worldTileManager = worldTileManager;
        _executor = executor;

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
            des.OnRequestDestruction += HandleTileObjectDestroyed;
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
            des.OnRequestDestruction -= HandleTileObjectDestroyed;
        }
    }

    private void HandleTileObjectDestroyed(GameObject obj)
    {
        foreach (var cell in _worldTileManager.GetAllCells())
        {
            if (cell.Object == obj)
            {
                cell.RemoveObject();
            }
        }
    }
}