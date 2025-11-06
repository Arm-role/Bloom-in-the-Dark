using System.Collections.Generic;
using UnityEngine;

public class TileTargetLogic
{
    private readonly GridConverter _gridConverter;
    public TileTargetLogic(GridConverter gridConverter)
    {
        _gridConverter = gridConverter;
    }

    public Vector3? GetTargetTile(Vector3 playerPos, Vector3 pointerPos, Vector3 forwardDir, float maxDistance)
    {
        Vector2Int targetCell = _gridConverter.WorldToGrid(pointerPos);
        Vector3 targetWorld = _gridConverter.GridToWorld(targetCell);
        float distance = Vector3.Distance(playerPos, targetWorld);

        if (distance <= maxDistance)
        {
            return targetWorld;
        }
        else
        {
            Vector3 forwardTileWorld = playerPos + forwardDir.normalized;
            Vector2Int frontCell = _gridConverter.WorldToGrid(forwardTileWorld);
            return _gridConverter.GridToWorld(frontCell);
        }
    }

    public List<TileInfo> GetTargetTilePreview(Vector3 playerPos, Vector3 pointerPos, Vector3 forwardDir, float maxDistance)
    {
        var target = GetTargetTile(playerPos, pointerPos, forwardDir, maxDistance);
        if (target == null) return new List<TileInfo>();

        return new List<TileInfo>
        {
            new TileInfo
            {
                WorldPosition = target.Value,
                State = Vector3.Distance(playerPos, target.Value) <= maxDistance
                    ? PlacementState.Valid
                    : PlacementState.OutOfRange
            }
        };
    }
}