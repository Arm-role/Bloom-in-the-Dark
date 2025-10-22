using UnityEngine;

public interface IItemBehavior
{
    PrimaryActionExecutionResult PrimaryActionExecute(IItemInstance itemInstance, Vector2 playerPosition, Vector2 pointerPosition, Collider2D target = null);
    SecondaryActionExecutionResult SecondaryActionExecute(IItemInstance itemInstance, Vector2 playerPosition, Vector2 pointerPosition, Collider2D target = null);
    DropExecutionResult DropExecute(IItemInstance itemInstance, Vector2 playerPosition, Vector2 pointerPosition, Collider2D target = null);
}