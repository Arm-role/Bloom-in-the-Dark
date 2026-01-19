public interface ITargetStrategy
{
    TargetResult Resolve(InteractionHandleContext context,
        ITargetingConfig config);
}
