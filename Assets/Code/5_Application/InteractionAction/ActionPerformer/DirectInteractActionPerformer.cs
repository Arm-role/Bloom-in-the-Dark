public class DirectInteractActionPerformer : IActionPerformer
{
    private CropPlacementSystem _cropPlacement;

    public DirectInteractActionPerformer(CropPlacementSystem cropPlacement)
    {
        _cropPlacement = cropPlacement;
    }

    public void Setup() { }

    public async void Execute(IDataProvider data)
    {
        if (data is not DirectInteractData directInteractData) return;
        if (directInteractData.PointerPosition.Value == null) return;

        var pointerPosition = directInteractData.PointerPosition.Value;
        if (directInteractData.ItemInstance.ItemData is SeedItem seedItem)
        {
            if (directInteractData.Target.IsTile)
            {
                var tileState = directInteractData.Target.TileState;

                await _cropPlacement.TryPlantAtWorld(seedItem.PlantName, pointerPosition, tileState);
            }
        }
    }
}