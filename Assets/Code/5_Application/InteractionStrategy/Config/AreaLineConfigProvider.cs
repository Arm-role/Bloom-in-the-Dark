public sealed class AreaLineConfigProvider
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

        return new AreaLineConfig()
        {
            Length = item.GetStat(EItemStatType.Range),
            Width = item.GetStat(EItemStatType.AreaRadius),
            XAngle = item.GetProperty<float>(EItemProperty.XAngle),
        };
    }
}