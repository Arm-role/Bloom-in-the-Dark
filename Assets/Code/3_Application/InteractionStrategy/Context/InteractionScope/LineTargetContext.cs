using UnityEngine;

public sealed class LineTargetContext : InteractionScope
{
    public Vector2 Origin { get; }
    public Vector2 Direction { get; }

    public override bool IsValid => Direction != Vector2.zero;

    public LineTargetContext(
        Vector2 origin,
        Vector2 direction)
    {
        Origin = origin;
        Direction = direction;
    }
}