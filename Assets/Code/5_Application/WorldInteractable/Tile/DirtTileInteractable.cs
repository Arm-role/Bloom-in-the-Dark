using System.Threading.Tasks;

public class DirtTileInteractable : TileInteractableAdapter
{
    private readonly TileBaseDataState _state;

    public DirtTileInteractable(TileBaseDataState state)
        : base(state, priority: 1f)
    {
        _state = state;
    }

    public override bool CanInteract(InteractionHandleContext ctx)
    {
        if (ctx.ItemInstance.Data is not ToolItem toolItem) return false;
        return toolItem.Name == "WateringItem";
    }

    public override Task<bool> TryInteract(InteractionHandleContext ctx)
    {
        if (!CanInteract(ctx)) return Task.FromResult(false);

        return Task.FromResult(true);
    }
}
