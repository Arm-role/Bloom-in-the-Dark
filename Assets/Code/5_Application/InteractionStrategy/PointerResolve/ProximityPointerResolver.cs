using UnityEngine;

public class ProximityPointerResolver : IPointerResolver
{
    private readonly ProximityIndicator _indicator;

    public ProximityPointerResolver(ProximityIndicator indicator)
    {
        _indicator = indicator;
    }

    public void Setup(InteractionHandleContext context)
    {
        if (!context.PlayerPosition.HasValue) return;
        if (context.ItemInstance is not ToolItem item) return;

        _indicator.Setup(item.AttackRange);   
        _indicator.UpdatePlayerPosition(context.PlayerPosition.Value);
    }

    public PointerResolveResult Resolve(InteractionHandleContext context)
    {
        if (!context.PlayerPosition.HasValue)
            return new PointerResolveResult(Vector2.zero, Vector2.zero, false);

        var pos = context.PlayerPosition.Value;

        return new PointerResolveResult(
            raw: pos,
            resolved: pos,
            valid: true
        );
    }
}
