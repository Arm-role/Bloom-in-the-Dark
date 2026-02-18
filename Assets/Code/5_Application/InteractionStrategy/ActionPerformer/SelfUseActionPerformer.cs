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
    Debug.Log("SelfUseActionPerformer");
    var item = intent.SourceItem;

    if (item != null)
    {
      _skillController.ActiveSelfSkill(item, intent);
      return InteractionResult.Consumed(null, null, ETargetType.Ally);
    }

    return InteractionResult.None;
  }
}