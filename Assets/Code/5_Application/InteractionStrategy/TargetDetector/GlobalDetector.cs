using UnityEngine;

public class GlobalDetector : ITargetDetector
{
    private readonly GlobalPointerResolver _pointerResolver;
    private readonly InteractionDispatcher _interactionDispatcher;

    public GlobalDetector(
        GlobalPointerResolver pointerResolver,
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
            _interactionDispatcher.TryInteract(context,
                resolvedPointer,
                ETargetResolveType.Interactable |
                ETargetResolveType.Ground,
                out var target);

            return new GlobalData(context.ItemInstance, resolvedPointer, target);
        }

        return new GlobalData();
    }
}