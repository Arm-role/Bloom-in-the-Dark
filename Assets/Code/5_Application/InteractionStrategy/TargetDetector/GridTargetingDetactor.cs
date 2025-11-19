
public class GridTargetingDetactor : ITargetDetector
{
    private readonly GridTargetingPointerResolver _pointerResolver;
    private readonly InteractionDispatcher _interactionDispatcher;

    public GridTargetingDetactor(
        GridTargetingPointerResolver pointerResolver,
        InteractionDispatcher interactionDispatcher)
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

        _interactionDispatcher.TryInteract(context,
            resolvedPointer,
            ETargetResolveType.Enemy |
            ETargetResolveType.Interactable |
            ETargetResolveType.Ground,
            out var target);

        if (target.IsTile)
        {
            return new GridTargetingData(context.ItemInstance, resolvedPointer, target.TileState);
        }

        return new GridTargetingData();
    }
}
