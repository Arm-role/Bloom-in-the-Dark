public interface ITargetingConfigProvider
{
    ITargetingConfig Create(
        InteractionHandleContext ctx);
}