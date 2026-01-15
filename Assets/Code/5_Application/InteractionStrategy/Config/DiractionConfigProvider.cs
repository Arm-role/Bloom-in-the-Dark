public sealed class DiractionConfigProvider
    : ITargetingConfigProvider
{
    public ITargetingConfig Create(
        InteractionHandleContext ctx)
    {
        var item = ctx.ItemInstance;

        if (item == null)
            return new DirectInteractConfig()
            {
                MaxDistance = 3,
            };
        
        if (!item.HasStat(EItemStatType.Range))
            return new DirectInteractConfig()
            {
                MaxDistance = 3,
            };

        return new DirectInteractConfig()
        {
            MaxDistance = item.GetStat(EItemStatType.Range),
        };
    }
}