using System.Threading.Tasks;
using UnityEngine;

public class MusmokeAction : IItemBehavior
{
    public PrimaryActionExecutionResult PrimaryActionExecute(
        IItemInstance itemInstance,
        Vector2 playerPosition,
        Vector2 pointerPosition,
        Collider2D target = null)
    {
        var result = new PrimaryActionExecutionResult();

        result.ShouldSpawnSelf = Task.FromResult(true);

        if (itemInstance.ItemData is PlantItem plantItem)
        {
            result.ParticleToPlay = plantItem.SkillName;
        }

        result.InventoryInteraction = (inventory) =>
        {
           bool canRemove =inventory.CanRemoveItem(itemInstance.ItemData, 1);
            if (canRemove)
            {
                int remaining = inventory.TryRemoveItem(itemInstance.ItemData, 1);
            }
        };

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
