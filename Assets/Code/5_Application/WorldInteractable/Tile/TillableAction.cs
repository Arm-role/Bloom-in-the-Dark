using System.Threading.Tasks;
using UnityEngine;

public class TillableAction : ITileAction
{
    private readonly TilemapService _tilemapService;

    public TillableAction(TilemapService tilemapService)
    {
        _tilemapService = tilemapService;
    }

    public bool CanInteract(InteractionHandleContext ctx, TileBaseDataState state)
    {
        if (ctx.ItemInstance.Data.Name == "Pickaxe") return true;

        return ctx.ItemInstance.Data is ToolItem tool
               && tool.Name == "Hoe"
               && !state.HasPlacedObject;
    }

    public Task<bool> TryInteract(InteractionHandleContext ctx, TileBaseDataState state)
    {
        var itemData = ctx.ItemInstance.Data;
        if (itemData.Name == "Pickaxe")
        {
            if (state.HasPlacedObject)
            {
                Debug.Log("DigPlant");

                var destructible = state.PlacedObject.GetComponent<IDestructible>();
                destructible.RequestDestruction();
            }
            else
            {
                Debug.Log("DigTile");

                _tilemapService.RemoveTile(state.WorldCenter, ETileLayerType.Overlay);
            }
        }
        else
        {
            _tilemapService.PlaceTile(state.CellPos, "Soil", ETileLayerType.Overlay);
        }

        return Task.FromResult(true);
    }
}
