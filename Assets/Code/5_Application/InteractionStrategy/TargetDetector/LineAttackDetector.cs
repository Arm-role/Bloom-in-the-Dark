public class LineAttackDetector : ITargetDetector
{
    private LinePointerResolver _pointerResolver;

    public LineAttackDetector(LinePointerResolver resolver)
    {
        _pointerResolver = resolver;
    }

    public void Setup(InteractionHandleContext ctx) { }

    public IDataProvider IntercationExcute(InteractionHandleContext ctx)
    {
        // ใช้ตำแหน่งผู้เล่น + ทิศของผู้เล่น
        var pointer = _pointerResolver.Resolve(ctx);

        var origin = pointer.RawPointer;
        var direction = pointer.ResolvedPointer;

        return new LineAttackData(ctx.ItemInstance, origin, direction);
    }
}