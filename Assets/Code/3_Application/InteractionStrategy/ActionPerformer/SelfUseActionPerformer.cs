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
  public async Task<InteractionExecutionPlan> Prepare(
    InteractionIntent intent,
    TargetResult target)
  {
    var item = intent.SourceItem;
    var skill = item.Data.Skill;

    if (!skill.Execute(item, out var payload))
      return null;

    return new InteractionExecutionPlan
    {
      Intent = intent,
      TargetMask = ETargetType.Ally,
      Commit = async () =>
      {
        _skillController.ActiveSelfSkill(payload, intent);

        var cooldown = new ItemCooldownFeedback(
            item.Data.Name,
            payload.Cooldown);

        Debug.Log("SelfUseActionPerformer");
        return InteractionResult.Consumed(
            null,
            null,
            ETargetType.Ally,
            cooldown);
      }
    };
  }
}