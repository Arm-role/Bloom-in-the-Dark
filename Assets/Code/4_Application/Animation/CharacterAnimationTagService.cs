public class CharacterAnimationTagService
{
  private readonly CharacterAnimationConfig _config;

  public CharacterAnimationTagService(CharacterAnimationConfig config)
  {
    _config = config;
  }

  public bool TryResolve(
      in InteractionExecutionResult result,
      out CharacterAnimationTag tag)
  {
    var intent = result.Intent;
    var item = intent.SourceItem;

    var category = item?.Data.Category ?? EItemCategory.None;
    var role = item?.Data.Role ?? EItemRole.None;
    var targetType = result.TargetMask;

    foreach (var rule in _config.Rules)
    {
      if (rule.MatchRule.Intent != intent.Type)
        continue;

      if ((rule.MatchRule.TargetMask & targetType) == 0)
        continue;

      if (rule.MatchRule.Category != EItemCategory.None &&
          rule.MatchRule.Category != category)
        continue;

      if (rule.MatchRule.ItemRole != EItemRole.None &&
          rule.MatchRule.ItemRole != role)
        continue;

      tag = rule.Tag;
      return true;
    }

    tag = null;
    return false;
  }
}