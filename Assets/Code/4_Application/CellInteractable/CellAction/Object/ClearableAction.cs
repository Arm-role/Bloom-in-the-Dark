using System.Threading.Tasks;

public class ClearableAction : ICellAction
{
  public InteractionStage Stage => InteractionStage.Pre;

  public ETargetType TargetType
  {
    get
    {
      if (_cachedState != null)
        return _cachedState.TargetType;

      return default;
    }
  }

  private ClearableState _cachedState;

  public Task<bool> CanProcess(InteractionIntent intent, IWorldCell cell)
  {
    if (cell is not WorldCell worldCell)
      return Task.FromResult(false);

    if (!worldCell.Object.TryGetComponent<ClearableState>(out var clearable))
      return Task.FromResult(false);

    if (clearable.RequiredIntent != intent.Type)
      return Task.FromResult(false);

    if (!clearable.CanBeClearedBy(intent.SourceItem))
      return Task.FromResult(false);

    _cachedState = clearable;

    return Task.FromResult(true);
  }

  public async Task<InteractionResult> Process(InteractionIntent intent, IWorldCell cell)
  {
    var result = new WorldAction();

    if (cell is not WorldCell worldCell)
      return InteractionResult.Blocked(cell, result, TargetType);

    var harvest = worldCell.Object.GetComponent<LootableObjectHandler>();

    ItemStack[] loot;

    if (intent.SourceItem is ToolItem tool)
      loot = harvest.GetHarvestLoot(tool);
    else
      loot = harvest.GetHarvestLoot();

    foreach (var stack in loot)
      result.ItemRewards.Add(stack);

    var interactionProfile = intent.SourceItem.Data.InteractionProfile;

    if (interactionProfile != null)
    {
      result.DamageTarget =
        interactionProfile.Damage;

      result.RewardCondition = ERewardCondition.OnObjectDestroyed;
    }

    return InteractionResult.Consumed(cell, result, TargetType, ItemCooldownFeedback.None);
  }
}