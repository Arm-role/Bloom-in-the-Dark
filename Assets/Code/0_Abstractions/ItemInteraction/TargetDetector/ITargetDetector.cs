public interface ITargetDetector
{
    void Setup(InteractionHandleContext context);
    IDataProvider IntercationExcute(InteractionHandleContext context);
}
