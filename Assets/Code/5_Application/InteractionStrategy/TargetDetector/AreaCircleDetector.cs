using UnityEngine;

public class AreaCircleDetector : ITargetDetector
{
    private readonly AreaCirclePointerResolver _pointerResolver;
    private readonly InteractionDispatcher _interactionDispatcher;

    public AreaCircleDetector(AreaCirclePointerResolver pointerResolver, InteractionDispatcher interactionDispatcher)
    {
        _pointerResolver = pointerResolver;
        _interactionDispatcher = interactionDispatcher;
    }

    public void Setup(InteractionHandleContext context)
    {
        _pointerResolver.Setup(context);
    }

    public IDataProvider IntercationExcute(InteractionHandleContext context)
    {
        var pointer = _pointerResolver.Resolve(context);

        var resolvedPointer = pointer.ResolvedPointer;

        _interactionDispatcher.TryInteract(context, resolvedPointer, ETargetResolveType.Ground, out var target);

        if (target.IsTile)
        {
            var tileTarget = target.TileState.GetTile(ETileLayerType.Ground);
            PlacementState state = tileTarget ? PlacementState.Valid : PlacementState.Blocked;

            return new AreaCircleData(context.ItemInstance, resolvedPointer, state);
        }

        return new AreaCircleData();
    }
}
