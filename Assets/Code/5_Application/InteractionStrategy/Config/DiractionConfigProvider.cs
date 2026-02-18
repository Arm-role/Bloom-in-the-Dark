public sealed class DiractionConfigProvider
    : ITargetingConfigProvider
{
    public ITargetingConfig Create(
        InteractionHandleContext ctx)
    {
        var itemInstance = ctx.ItemInstance;
        if (itemInstance == null)
            return new DirectInteractConfig(2);

        var data = itemInstance.Data;
        if (data == null)
            return null;

        if (data.InteractionProfile == null)
            return null;

        return new DirectInteractConfig(data.InteractionProfile.Range);
    }
}