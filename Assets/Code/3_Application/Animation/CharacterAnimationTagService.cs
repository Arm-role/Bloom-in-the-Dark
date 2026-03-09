public class CharacterAnimationTagService
{
  private readonly CharacterAnimationConfig _config;

  public CharacterAnimationTagService(CharacterAnimationConfig config)
  {
    _config = config;
  }

  public bool TryResolve(
      in AnimationRequest request,
      out AnimationTag result)
  {
    var intent = request.Intent; 
    var item = request.ItemDefinition;
    var targetType = request.TargetMask;
    
    foreach (var entry in _config.Rules)
    {
      var rule = entry.MatchRule;

      if (!rule.Match(intent, item.Tags, targetType))
        continue;

      result = entry.Tag;
      return true;
    }

    result = null;
    return false;
  }
}