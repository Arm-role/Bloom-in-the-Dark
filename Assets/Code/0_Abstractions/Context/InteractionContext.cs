using UnityEngine;

public readonly struct InteractionContext
{
    public readonly InputActionType ActiveActions;
    public readonly InputActionType ReleasedActions;

    public readonly bool UseSourceItem;
    public readonly Vector2? LastPointerPosition;

    public readonly bool IsSkillModifierHeld; 
    public readonly bool IsDashPressed;       
    public readonly bool IsInventoryToggle;   

    public InteractionContext(
        InputActionType activeActions = InputActionType.None,
        InputActionType releasedActions = InputActionType.None,
        bool useSourceItem = false,
        Vector2? lastPointerPosition = null,
         bool isSkillModifierHeld = false,
        bool isDashPressed = false,
        bool isInventoryToggle = false)
    {
        ActiveActions = activeActions;
        ReleasedActions = releasedActions;

        UseSourceItem = useSourceItem;
        LastPointerPosition = lastPointerPosition;

        IsSkillModifierHeld = isSkillModifierHeld;
        IsDashPressed = isDashPressed;
        IsInventoryToggle = isInventoryToggle;
    }
}
