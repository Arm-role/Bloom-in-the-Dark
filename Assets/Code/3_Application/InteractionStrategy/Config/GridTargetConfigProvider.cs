
public sealed class GridTargetConfigProvider 
    : ITargetingConfigProvider
{
    public ITargetingConfig Create(
        InteractionHandleContext ctx)
    {
        var itemInstance = ctx.ItemInstance;
        if (itemInstance == null)
            return null;

        var data = itemInstance.Data;
        if (data == null)
            return null;

        if (data.InteractionProfile == null)
            return null;
        
        if (data.PlacementProfile == null)
            return null;

        return new GridTargetConfig(
            data.PlacementProfile.GridSize,
            data.InteractionProfile.Range);
    }
}