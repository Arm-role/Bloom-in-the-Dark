using System.Threading.Tasks;
using UnityEngine;

public class PlacementActionPerformer : IActionPerformer
{
    private readonly SpawnerHandle _spawner;
    private readonly WorldTileManager _tileManager;

    public PlacementActionPerformer(SpawnerHandle spawner, WorldTileManager tileManager)
    {
        _spawner = spawner;
        _tileManager = tileManager;
    }

    public bool CanExecute(InteractionIntent intent, TargetResult target)
    {
        return target.IsValid
            && intent.SourceItem?.Data?.PlacementProfile?.ObjectKey != null;
    }

    public Task<InteractionExecutionPlan> Prepare(
        GameObject owner,
        InteractionIntent intent,
        TargetResult target)
    {
        int spawnId = intent.SourceItem.Data.PlacementProfile.ObjectKey.RuntimeTag.Hash;
        Vector3 spawnPos = ResolveCenterFromCells(target);

        var plan = new InteractionExecutionPlan
        {
            Intent = intent,
            TargetMask = ETargetType.Ground,
            Commit = async () =>
            {
                var go = await _spawner.SpawnAsync(spawnId, spawnPos);

                if (go == null)
                    return InteractionResult.None;

                bool placed = _tileManager.TryPlaceObject(go);

                if (!placed)
                {
                    _spawner.Despawn(go);
                    return InteractionResult.None;
                }

                return InteractionResult.Consumed(
                    null,
                    new WorldAction(),
                    ETargetType.None,
                    ItemCooldownFeedback.None,
                    InteractionCost.OneItem);
            }
        };

        return Task.FromResult(plan);
    }

    private static Vector3 ResolveCenterFromCells(TargetResult target)
    {
        if (target.Cells == null || target.Cells.Count == 0)
            return target.Extra is Vector2 v2 ? (Vector3)v2 : (Vector3)target.Origin;

        Vector3 sum = Vector3.zero;
        foreach (var cell in target.Cells)
            sum += cell.WorldCenter;
        return sum / target.Cells.Count;
    }
}
