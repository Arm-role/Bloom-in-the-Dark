using System.Threading.Tasks;

public class DirectInteractActionPerformer : IActionPerformer
{
    private CropPlacementSystem _cropPlacement;

    public DirectInteractActionPerformer(CropPlacementSystem cropPlacement)
    {
        _cropPlacement = cropPlacement;
    }

    public void Setup() { }

    public async Task<bool> Execute(InteractionHandleContext context, IDataProvider data)
    {
        if (data is not DirectInteractData directInteractData) return await Task.FromResult(false);
        if (directInteractData.PointerPosition.Value == null) return await Task.FromResult(false);

        var pointerPosition = directInteractData.PointerPosition.Value;
        if (directInteractData.ItemInstance.ItemData is SeedItem seedItem)
        {
            if (directInteractData.Target.IsTile)
            {
                var tileState = directInteractData.Target.TileState;

                return await _cropPlacement.TryPlantAtWorld(seedItem.PlantName, pointerPosition, tileState);
            }
        }

        return await Task.FromResult(false);
    }
}