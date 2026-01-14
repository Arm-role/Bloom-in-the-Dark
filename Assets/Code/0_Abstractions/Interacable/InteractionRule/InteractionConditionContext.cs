public struct InteractionConditionContext
{
    // Raw input state
    public InputActionType Pressed;
    public InputActionType Held;
    public InputActionType Released;

    // System state
    public bool IsSkillPreviewActive;
    public bool IsInventoryOpen;
    public bool IsDraggingUI;

    // Item state
    public IItemInstance Item;
}