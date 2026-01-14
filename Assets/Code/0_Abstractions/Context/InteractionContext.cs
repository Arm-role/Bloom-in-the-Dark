using UnityEngine;

public readonly struct InteractionContext
{
    public readonly InputActionType Pressed;
    public readonly InputActionType Held;
    public readonly InputActionType Released;

    public readonly bool UseSourceItem;
    public readonly Vector2? LastPointerPosition;

    public readonly bool IsSkillModifierHeld;
    public readonly bool IsDashPressed;
    public readonly bool IsInventoryToggle;

    public InteractionContext(
        InputActionType pressed = InputActionType.None,
        InputActionType held = InputActionType.None,
        InputActionType released = InputActionType.None,
        bool useSourceItem = false,
        Vector2? lastPointerPosition = null,
        bool isSkillModifierHeld = false,
        bool isDashPressed = false,
        bool isInventoryToggle = false)
    {
        Pressed = pressed;
        Held = held;
        Released = released;

        UseSourceItem = useSourceItem;
        LastPointerPosition = lastPointerPosition;

        IsSkillModifierHeld = isSkillModifierHeld;
        IsDashPressed = isDashPressed;
        IsInventoryToggle = isInventoryToggle;
    }
}