using UnityEngine;

public abstract class InteractionScope
{
    public abstract bool IsValid { get; }
    public virtual Vector2? PointerPosition { get; }
    public static InteractionScope Invalid => default;
}