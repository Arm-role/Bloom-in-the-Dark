using UnityEngine;

public class GridTargetingPointerResolver : IPointerResolver
{
    private readonly TileTargetLogic _clampTarget;

    public GridTargetingPointerResolver(TileTargetLogic clampTarget)
    {
        _clampTarget = clampTarget;
    }

    public void Setup(InteractionHandleContext context)
    {
        if (context.ItemInstance.Data is not ToolItem item) return;

        _clampTarget.Setup(context.PlayerPosition.Value, item.AttackRange);
        _clampTarget.UpdateState(context.PlayerPosition.Value, context.PlayerDirection.Value);
    }

    public PointerResolveResult Resolve(InteractionHandleContext context)
    {
        if (!context.PlayerPosition.HasValue || !context.PointerPosition.HasValue)
            return new PointerResolveResult(Vector2.zero, Vector2.zero, false);

        Vector2 rawPointer = context.PointerPosition.Value;
        Vector2 playerDir = context.PlayerDirection.Value;
        Vector2 resolvedPointer = _clampTarget.ClampTarget(rawPointer, playerDir);

        return new PointerResolveResult(rawPointer, resolvedPointer, true);
    }
}
