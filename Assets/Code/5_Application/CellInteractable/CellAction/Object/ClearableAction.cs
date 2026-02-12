using System.Threading.Tasks;
using UnityEngine;

public class ClearableAction : ICellAction
{
  public InteractionStage Stage => InteractionStage.Pre;

  public Task<bool> CanProcess(InteractionIntent intent, IWorldCell cell)
  {
    if (cell is not WorldCell worldCell)
      return Task.FromResult(false);

    if (!worldCell.Object.TryGetComponent<ClearableState>(out var clearable))
      return Task.FromResult(false);

    if (clearable.RequiredIntent != intent.Type)
      return Task.FromResult(false);

    if (clearable.ToolName != intent.SourceItem.Data.Name)
      return Task.FromResult(false);
    
    return Task.FromResult(true);
  }

  public async Task<InteractionResult> Process(InteractionIntent intent, IWorldCell cell)
  {
    var result = new WorldAction();

    if (cell is not WorldCell worldCell)
      return InteractionResult.Blocked(result);

    var harvest = worldCell.Object.GetComponent<LootableObjectHandler>();

    ItemStack[] loot;

    if (intent.SourceItem is ToolItem tool)
      loot = harvest.GetHarvestLoot(tool);
    else
      loot = harvest.GetHarvestLoot();

    foreach (var stack in loot)
      result.ItemRewards.Add(stack);

    if (intent.SourceItem.HasStat(EItemStatType.DamageOnInteract))
    {
      result.DamageTarget =
        intent.SourceItem.GetStat(EItemStatType.DamageOnInteract);

      result.RewardCondition = ERewardCondition.OnObjectDestroyed;
    }

    return InteractionResult.Consumed(result);
  }
}