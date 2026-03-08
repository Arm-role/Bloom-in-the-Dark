public interface IHoldGestureResolver
{
    InputActionType Resolve(InputSnapshot snap, float deltaTime);
    void Reset();
}