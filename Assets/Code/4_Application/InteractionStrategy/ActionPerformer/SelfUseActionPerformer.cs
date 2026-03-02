using System.Threading.Tasks;
using UnityEngine;

public sealed class SelfUseActionPerformer : IActionPerformer
{
  private readonly SkillController _skillController;

  public SelfUseActionPerformer(SkillController skillController)
  {
    _skillController = skillController;
  }

  public bool CanExecute(
    InteractionIntent intent,
    TargetResult target)
  {
    return intent.SourceItem != null && target.IsValid;
  }

  public async Task<InteractionResult> Execute(
    InteractionIntent intent,
    TargetResult target)
  {

    var item = intent.SourceItem;
    var skill = item.Data.Skill;

    if (!skill.Execute(item, out var payload))
      return InteractionResult.None;

    _skillController.ActiveSelfSkill(payload, intent);

    var itemCooldown = new ItemCooldownFeedback(item.Data.Name, payload.Cooldown);
    return InteractionResult.Consumed(null, null, ETargetType.Ally, itemCooldown);
  }
}