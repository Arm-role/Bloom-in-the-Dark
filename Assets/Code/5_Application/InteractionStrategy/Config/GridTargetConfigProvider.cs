using UnityEngine;

public sealed class GridTargetConfigProvider 
    : ITargetingConfigProvider
{
    public ITargetingConfig Create(
        InteractionHandleContext ctx)
    {
        var item = ctx.ItemInstance;

        if (!item.HasStat(EItemStatType.Range) && 
            !item.HasProperty(EItemProperty.GridSize))
            return null;
        
        return new GridTargetConfig
        {
            Size = item.GetProperty<Vector2Int>(EItemProperty.GridSize),
            MaxRange = item.GetStat(EItemStatType.Range)
        };
    }
}