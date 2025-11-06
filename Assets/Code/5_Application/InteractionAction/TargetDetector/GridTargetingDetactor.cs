using System.Collections.Generic;
using UnityEngine;

public class GridTargetingDetactor : ITargetDetector
{
    private readonly TileTargetLogic _tileLogic;
    private readonly float _maxDistance;

    public GridTargetingDetactor(TileTargetLogic tileLogic, float maxDistance = 2f)
    {
        _tileLogic = tileLogic;
        _maxDistance = maxDistance;
    }

    public void Setup(InteractionHandleContext context) { }

    public IDataProvider IntercationExcute(InteractionHandleContext context)
    {
        var playerPos = context.PlayerPosition.Value;
        var pointerPos = context.PointerPosition.Value;
        var playerDirection = context.PlayerDirection.Value;

        var targetTile = _tileLogic.GetTargetTile(playerPos, pointerPos, playerDirection, _maxDistance);

        var tileInfos = new List<TileInfo>
        {
            new TileInfo
            {
                WorldPosition = targetTile.Value,
                State = Vector3.Distance(playerPos, targetTile.Value) <= _maxDistance
                    ? PlacementState.Valid
                    : PlacementState.OutOfRange
            }
        };

        if (targetTile == null)
            return null;

        return new GridTargetingData(context.ItemInstance, tileInfos);
    }
}
