using System.Threading.Tasks;
using UnityEngine;

public class WaterableAction : ITileAction
{
    public bool CanInteract(InteractionHandleContext ctx, TileBaseDataState state)
    {
        return ctx.ItemInstance.Data is ToolItem tool
               && tool.Name == "WateringItem";
    }

    public Task<bool> TryInteract(InteractionHandleContext ctx, TileBaseDataState state)
    {
        Debug.Log("Watering soil");

        state.IsWatered = true;
        state.UpdateVisualState();

        return Task.FromResult(true);
    }
}