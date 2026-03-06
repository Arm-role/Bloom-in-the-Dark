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

  public async Task<InteractionExecutionPlan> Prepare(
    InteractionIntent intent,
    TargetResult target)
  {
    var item = intent.SourceItem;
    var skill = item.Data.Skill;

    if (!skill.Execute(item, out var payload))
      return null;

    Vector2 targetPos = target.Extra != null
        ? (Vector2)target.Extra
        : target.Origin;

    return new InteractionExecutionPlan
    {
      Intent = intent,
      TargetMask = ETargetType.Enemy,
      Commit = async () =>
      {
        _skillController.ActiveSkill(
            payload,
            intent,
            item.Data.Skill,
            targetPos,
            target.Direction);

        var cooldown = new ItemCooldownFeedback(
            item.Data.Name,
            payload.Cooldown);

        return InteractionResult.Consumed(
            null,
            null,
            ETargetType.Enemy,
            cooldown);
      }
    };
  }
}