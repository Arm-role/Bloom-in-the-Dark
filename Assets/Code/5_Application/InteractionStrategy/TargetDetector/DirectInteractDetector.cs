public class DirectInteractDetector : ITargetDetector
{
    private readonly DirectInteractPointerResolver _pointerResolver;
    private readonly InteractionDispatcher _interactionDispatcher;

    public DirectInteractDetector(
        DirectInteractPointerResolver pointerResolver,
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

        if (pointer.IsValid)
        {
            _interactionDispatcher.TryInteract(context, resolvedPointer, ETargetResolveType.Ground, out var target);
            return new DirectInteractData(context.ItemInstance, resolvedPointer, target);
        }

        return new DirectInteractData();
    }
}