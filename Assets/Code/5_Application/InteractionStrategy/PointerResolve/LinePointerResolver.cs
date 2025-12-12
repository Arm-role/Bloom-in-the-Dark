public class LinePointerResolver : IPointerResolver
{
    public void Setup(InteractionHandleContext context) { }
   
    public PointerResolveResult Resolve(InteractionHandleContext context)
    {
        var playerPos = context.PlayerPosition.Value;
        var dir = context.PointerPosition.Value - context.PlayerPosition.Value;

        return new PointerResolveResult(playerPos, dir, true);
    }
}
