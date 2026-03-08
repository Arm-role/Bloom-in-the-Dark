public class CharacterAnimationTagService
{
  private readonly CharacterAnimationConfig _config;

  public CharacterAnimationTagService(CharacterAnimationConfig config)
  {
    _config = config;
  }

  public bool TryResolve(
      in AnimationRequest result,
      out AnimationTag tag)
  {
    var targetType = result.TargetMask;
    var category = result.Category; 
    var role = result.Role;
    
    foreach (var rule in _config.Rules)
    {
      if (rule.MatchRule.Intent != result.Intent)
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