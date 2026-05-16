using System.Threading.Tasks;
using UnityEngine;

public class DemolishBuildingAction : ICellAction
{
  public InteractionStage Stage => InteractionStage.Pre;
  public ETargetType TargetType => ETargetType.Interactable;

  public Task<bool> CanProcess(InteractionIntent intent, IWorldCell cell)
  {
    if (intent.Type != EInteractionIntentType.Demolish)
      return Task.FromResult(false);

    if (!intent.HasItem || !intent.SourceItem.Data.HasTag(TagLibrary.Get("Tool.Hammer")))
      return Task.FromResult(false);

    if (cell is not WorldCell worldCell)
      return Task.FromResult(false);

    var controller = worldCell.Object?.GetComponent<BaseBuildingController>();
    Debug.Log("controller is null: " + (controller == null) + " " + controller.IsAlive);
    return Task.FromResult(controller != null && controller.IsAlive);
  }

  public Task<InteractionResult> Process(InteractionIntent intent, IWorldCell cell)
  {
    var result = new WorldAction();

    Debug.Log("cell is not WorldCell worldCell");

    if (cell is not WorldCell worldCell)
      return Task.FromResult(InteractionResult.Blocked(cell, result, TargetType));

    Debug.Log("controller == null");

    var controller = worldCell.Object.GetComponent<BaseBuildingController>();
    if (controller == null)
      return Task.FromResult(InteractionResult.Blocked(cell, result, TargetType));

    var profile = intent.SourceItem.Data.InteractionProfile;
    int damage = profile != null ? Mathf.Max(1, (int)profile.Damage) : 1;
    var direction = intent.Direction ?? Vector2.zero;

    var lootable = worldCell.Object.GetComponent<ILootableHandler>();

    var ctx = new DamageContext(null, intent, damage, direction, 0f, 0f);
    bool destroyed = controller.TakeDamage(ctx);

    if (destroyed)
    {
      if (lootable != null)
      {
        var loot = intent.SourceItem.Data.HasTag(TagLibrary.Get("Tool"))
          ? lootable.GetHarvestLoot(intent.SourceItem.Data)
          : lootable.GetHarvestLoot();

        result.Exp = loot.Exp;
        result.RewardCondition = ERewardCondition.Immediate;
        foreach (var stack in loot.Item2)
          result.ItemRewards.Add(stack);
      }
    }

    return Task.FromResult(
        InteractionResult.Consumed(cell, result, TargetType, ItemCooldownFeedback.None));
  }
}
