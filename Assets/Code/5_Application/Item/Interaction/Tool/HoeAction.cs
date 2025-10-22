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

        result.InteractionHandle = (handle) =>
        {
            handle.IntercationExcute(itemInstance, playerPosition, pointerPosition, target);
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
