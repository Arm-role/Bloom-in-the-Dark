using System.Threading.Tasks;
using UnityEngine;

public class SkillActionPerformer : IActionPerformer
{
  private readonly SkillController _skillController;

  public SkillActionPerformer(
      SkillController skillController)
  {
    _skillController = skillController;
  }

  public bool CanExecute(
      InteractionIntent intent,
      TargetResult target)
  {
    return target.Extra is Vector2 || target.Extra == null;
  }

  public async Task<InteractionResult> Execute(
      InteractionIntent intent,
      TargetResult target)
  {
    var targetPos = (target.Extra != null) ?
        (Vector2)target.Extra :
        target.Origin;

    var item = intent.SourceItem;
    var skill = item.Data.Skill;

    if (!skill.Execute(item, out var payload))
      return InteractionResult.None;

      if (target.Direction != Vector2.zero)
      _skillController.ActiveSkill(
          payload,
          intent,
          item.Data.Skill,
          targetPos,
          target.Direction);
    else
      _skillController.ActiveSkill(
          payload,
          intent,
          item.Data.Skill,
          targetPos);

    var itemCooldown = new ItemCooldownFeedback(item.Data.Name, payload.Cooldown);
    return InteractionResult.Consumed(null, null, ETargetType.Enemy, itemCooldown);
  }
}