using UnityEngine;

public class GlobalPointerResolver : IPointerResolver
{
    private float _maxDistance;

    public GlobalPointerResolver(float maxDistance)
    {
        _maxDistance = maxDistance;
    }

    public void Setup(InteractionHandleContext context) { }

    public PointerResolveResult Resolve(InteractionHandleContext context)
    {
        if (!context.PlayerPosition.HasValue || !context.PointerPosition.HasValue)
            return new PointerResolveResult(Vector2.zero, Vector2.zero, false);

        Vector2 playerPosition = context.PlayerPosition.Value;
        Vector2 rawPointer = context.PointerPosition.Value;

        float distance = Vector2.Distance(playerPosition, rawPointer);

        if (distance < _maxDistance)
        {
            return new PointerResolveResult(rawPointer, rawPointer, true);
        }

        return new PointerResolveResult(rawPointer, rawPointer, false);
    }
}