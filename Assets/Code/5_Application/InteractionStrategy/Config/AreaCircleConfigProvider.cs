public sealed class AreaCircleConfigProvider
    : ITargetingConfigProvider
{
    public ITargetingConfig Create(
        InteractionHandleContext ctx)
    {
        var item = ctx.ItemInstance;

        if (!item.HasStat(EItemStatType.Range) && 
            !item.HasStat(EItemStatType.AreaRadius) &&
            !item.HasProperty(EItemProperty.XAngle))
            return null;

        return new AreaCircleConfig
        {
            Range = item.GetStat(EItemStatType.Range),
            Radius = item.GetStat(EItemStatType.AreaRadius),
            XAngle = item.GetProperty<float>(EItemProperty.XAngle),
        };
    }
}