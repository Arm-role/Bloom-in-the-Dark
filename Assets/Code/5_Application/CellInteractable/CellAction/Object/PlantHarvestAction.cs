using System.Threading.Tasks;

public class PlantHarvestAction : ICellAction
{
    public InteractionStage Stage => InteractionStage.Pre;

    public Task<bool> CanProcess(InteractionIntent intent, IWorldCell cell)
    {
        if (cell is not WorldCell worldCell)
            return Task.FromResult(false);

        var plantState = worldCell.PlacedObject.GetComponent<PlantState>();
        return Task.FromResult(
            plantState != null &&
            plantState.IsGrown);
    }

    public async Task<InteractionResult> Process(InteractionIntent intent, IWorldCell cell)
    {
        var result = new WorldAction();

        if (cell is not WorldCell worldCell)
            return InteractionResult.Blocked(result);

        var harvest = worldCell.PlacedObject.GetComponent<PlantHarvestHandler>();

        ItemStack[] loot;

        if (intent.SourceItem is ToolItem tool)
            loot = harvest.GetHarvestLoot(tool);
        else
            loot = harvest.GetHarvestLoot();

        foreach (var stack in loot)
            result.ItemRewards.Add(stack);

        result.RemoveObject = true;
        return InteractionResult.Consumed(result);
    }
}