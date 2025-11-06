
public class SkillAreaCircleAction : IItemBehavior
{
    public ActionExecutionResult ActionExecute(InteractionHandleContext context)
    {
        var result = new ActionExecutionResult();
        var Decide1 = new ProcessState<bool, bool>();
        var Decide2 = new ProcessState<IDataProvider, bool>();

        result.ModifierInput = (auxiliaryInput) =>
        {
            Decide1.Source = auxiliaryInput.ExecuteActions == InputActionType.SkillModifierHeld;
        };

        result.TargetDetector = (handle) =>
        {
            if (Decide1.Source)
            {
                Decide2.Source = handle.IntercationExcute(context);
            }
        };

        result.InventoryInteraction = (inventory) =>
        {
            bool canRemove = inventory.CanRemoveItem(context.ItemInstance.ItemData, 1);
            if (Decide2.Source != null && canRemove)
            {
                if (Decide2.Source is AreaCircleData data && data.State != null)
                {
                    Decide2.Target = data.State.Value == PlacementState.Valid;
                    int remaining = inventory.TryRemoveItem(context.ItemInstance.ItemData, 1);
                }
            }
        };

        result.PlayerData = (playerData) =>
        {
            var dir = (context.PointerPosition.Value - context.PlayerPosition.Value).normalized;
            playerData.Look(dir);
        };

        result.ActionPerformer = (action) =>
        {
            if (Decide2.Source != null && Decide2.Target)
            {
                action.Execute(Decide2.Source);
            }
        };

        return result;
    }
    public SecondaryActionExecutionResult SecondaryActionExecute(InteractionHandleContext context)
    {
        return new SecondaryActionExecutionResult();
    }
    public DropExecutionResult DropExecute(InteractionHandleContext context)
    {
        return new DropExecutionResult();
    }
}

