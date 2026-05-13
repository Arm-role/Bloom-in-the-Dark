using UnityEngine;

public struct InputSnapshot
{
    public InputActionType Pressed;
    public InputActionType Held;
    public InputActionType Released;

    public Vector2 PointerPosition;
}