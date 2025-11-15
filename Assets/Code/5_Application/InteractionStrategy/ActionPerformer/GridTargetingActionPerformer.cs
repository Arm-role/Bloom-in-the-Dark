using System.Threading.Tasks;

public class GridTargetingActionPerformer : IActionPerformer
{
    private readonly TileLibrary _tileLibrary;
    private readonly TilemapService _tilemapService;

    public GridTargetingActionPerformer(TileLibrary tileLibrary, TilemapService tilemapService)
    {
        _tileLibrary = tileLibrary;
        _tilemapService = tilemapService;
    }

    public void Setup()
    {
    }

    public Task<bool> Execute(InteractionHandleContext context, IDataProvider data)
    {
        if (data is not GridTargetingData gridData)
            return Task.FromResult(false);

        if (gridData.ItemInstance.ItemData is ToolItem toolItem)
        {
            if (toolItem.Name == "Hoe")
            {
                var soilTile = _tileLibrary.GetTileDataByName("Soil");
                _tilemapService.PlaceTile(data.PointerPosition.Value, soilTile);

                return Task.FromResult(true);
            }
            else if (toolItem.Name == "Pickaxe")
            {
                _tilemapService.RemoveTile(data.PointerPosition.Value);
                return Task.FromResult(true);
            }
        }

        return Task.FromResult(false);
    }
}
