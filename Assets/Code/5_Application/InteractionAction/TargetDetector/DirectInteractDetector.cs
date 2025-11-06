using System.Linq;
using UnityEngine;

public class DirectInteractDetector : ITargetDetector
{
    private readonly InteractionTargetResolver _interactionTarget;
    private float _maxDistance;
    public DirectInteractDetector(InteractionTargetResolver interactionTarget, float maxDistance)
    {
        _interactionTarget = interactionTarget;
        _maxDistance = maxDistance;
    }

    public void Setup(InteractionHandleContext context)
    {
    }

    public IDataProvider IntercationExcute(InteractionHandleContext context)
    {
        var playerPosition = context.PlayerPosition.Value;
        var pointerPosition = context.PointerPosition.Value;

        float distance = Vector2.Distance(playerPosition, pointerPosition);

        if (distance < _maxDistance)
        {
            _interactionTarget.TryResolveTarget(pointerPosition, out var target);
            if (!target.IsTile) return null;

            if (context.ItemInstance.ItemData is SeedItem seed)
            {
                foreach (var tileName in seed.ReplaceableTiles)
                {
                    if (tileName == target.TileData.DisplayName)
                    {
                        return new DirectInteractData(context.ItemInstance, pointerPosition, target);
                    }
                }
            }
        }

        return null;
    }
}