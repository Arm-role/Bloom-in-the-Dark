public class InstantHoldResolver : IHoldGestureResolver
{
    private readonly InputActionType _action;

    public InstantHoldResolver(InputActionType action)
    {
        _action = action;
    }

    public InputActionType Resolve(InputSnapshot snap, float deltaTime)
    {
        return snap.Held.HasFlag(_action) ? _action : InputActionType.None;
    }

    public void Reset() { }
}