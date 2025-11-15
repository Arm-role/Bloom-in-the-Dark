public interface IPointerResolver
{
    void Setup(InteractionHandleContext context);
    PointerResolveResult Resolve(InteractionHandleContext context);
}