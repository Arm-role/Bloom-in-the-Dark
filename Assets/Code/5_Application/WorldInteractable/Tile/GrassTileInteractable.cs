using System.Threading.Tasks;
using UnityEngine;

public class GrassTileInteractable : TileInteractableAdapter
{
    private readonly TileBaseDataState _state;
    private readonly TilemapService _tilemapService;

    public GrassTileInteractable(TileBaseDataState state, TilemapService tilemapService)
        : base(state, priority: 1f)
    {
        _state = state;
        _tilemapService = tilemapService;
    }

    public override bool CanInteract(InteractionHandleContext ctx)
    {
        Debug.Log(ctx.ItemInstance.Data.Name);
        if (ctx.ItemInstance.Data is not ToolItem toolItem) return false;
        return toolItem.Name == "Hoe";
    }

    public override Task<bool> TryInteract(InteractionHandleContext ctx)
    {
        if (!CanInteract(ctx)) return Task.FromResult(false);

        _tilemapService.PlaceTile(_state.CellPos, "Soil", ETileLayerType.Overlay);

        return Task.FromResult(true);
    }
}