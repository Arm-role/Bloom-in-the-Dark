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

        result.ItemAction = (action) =>
        {
            action.Action(pointerPosition);
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
