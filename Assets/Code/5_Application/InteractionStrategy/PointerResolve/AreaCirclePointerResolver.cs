using UnityEngine;

public class AreaCirclePointerResolver : IPointerResolver
{
    private readonly AreaCircleIndicator _indicator;

    public AreaCirclePointerResolver(AreaCircleIndicator indicator)
    {
        _indicator = indicator;
    }
    public void Setup(InteractionHandleContext context)
    {
        if (context.ItemInstance.ItemData is not PlantItem item) return;

        _indicator.Setup(item.Range, item.AreaRadius);
        _indicator.UpdatePlayerPosition(context.PlayerPosition.Value);
    }

    public PointerResolveResult Resolve(InteractionHandleContext context)
    {
        if (!context.PlayerPosition.HasValue || !context.PointerPosition.HasValue)
            return new PointerResolveResult(Vector2.zero, Vector2.zero, false);

        Vector2 rawPointer = context.PointerPosition.Value;
        Vector2 resolvedPointer = _indicator.ClampPoint(rawPointer);

        return new PointerResolveResult(rawPointer, resolvedPointer, true);
    }
}
