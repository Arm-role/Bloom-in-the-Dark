using UnityEngine;

public class HoeAction : IItemBehavior
{
    public PrimaryActionExecutionResult PrimaryActionExecute(
        IItemInstance itemInstance,
        Vector2 playerPosition,
        Vector2 pointerPosition,
        Collider2D target = null)
    {
        var result = new PrimaryActionExecutionResult();
        var process = new ProcessState<IDataProvider, bool>();

        result.InteractionHandle = (handle) =>
        {
            process.Source = handle.IntercationExcute(new InteractionHandleContext(itemInstance, playerPosition, pointerPosition, target));
        };

        result.ItemAction = (action) =>
        {
            if (process.Source != null)
            {
                action.Execute(process.Source);
            }
        }; 

        Debug.Log("HoeAction");
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
