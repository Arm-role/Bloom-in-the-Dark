using System.Threading.Tasks;

public class TillGrassToSoilAction : ICellAction
{
    public InteractionStage Stage => InteractionStage.Main;
    private readonly IBaseTileData _baseTileData;

    public TillGrassToSoilAction(
        IBaseTileData tileData)
    {
        _baseTileData = tileData;
    }

    public Task<bool> CanProcess(
        InteractionIntent intent,
        IWorldCell cell)
    {
        if (cell.HasPlacedObject)
            return Task.FromResult(false);

        if (!intent.Is(EInteractionIntentType.Dig))
            return Task.FromResult(false);

        if (!(intent.SourceItem.Data.Name == "Hoe"))
            return Task.FromResult(false);

        return Task.FromResult(true);
    }

    public Task<InteractionResult> Process(
        InteractionIntent intent,
        IWorldCell cell)
    {
        var result = new WorldAction
        {
            AddTile = _baseTileData,
            TileTargetLayer = ETileLayerType.Overlay
        };
        
        return Task.FromResult(InteractionResult.Consumed(result));
    }
}