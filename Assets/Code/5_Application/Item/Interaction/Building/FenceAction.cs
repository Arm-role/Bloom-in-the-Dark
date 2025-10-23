using System.Threading.Tasks;
using UnityEngine;

public class FenceAction : IItemBehavior
{
    public PrimaryActionExecutionResult PrimaryActionExecute(
        IItemInstance itemInstance,
        Vector2 playerPosition,
        Vector2 pointerPosition,
        Collider2D target = null)
    {
        var result = new PrimaryActionExecutionResult();
        var process = new ProcessState<bool, bool>();

        result.InteractionHandle = (handle) =>
        {
            process.Source = handle.IntercationExcute(new InteractionHandleContext(itemInstance, playerPosition, pointerPosition, target));
        };

        result.InventoryInteraction = (inventory) =>
         {
             process.Target = inventory.CanRemoveItem(itemInstance.ItemData, 1);
             if (process.Source && process.Target)
             {
                 int remaining = inventory.TryRemoveItem(itemInstance.ItemData, 1);
             }
         };

        result.ItemAction = (action) =>
        {
            if (process.Source && process.Target)
            {
                action.Action(pointerPosition);
            }
        };


        Debug.Log("FenceAction");
        return result;
    }

    public SecondaryActionExecutionResult SecondaryActionExecute(
        IItemInstance itemInstance,
        Vector2 playerPosition,
        Vector2 pointerPosition,
        Collider2D target = null)
    {
        return new SecondaryActionExecutionResult();
    }
    public DropExecutionResult DropExecute(
        IItemInstance itemInstance,
        Vector2 playerPosition,
        Vector2 pointerPosition,
        Collider2D target = null)
    {
        return new DropExecutionResult();
    }
}
