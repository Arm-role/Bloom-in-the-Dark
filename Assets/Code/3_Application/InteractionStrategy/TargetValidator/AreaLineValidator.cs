
public class AreaLineValidator : ITargetValidator
{
  public ValidationResult Validate(
    InteractionHandleContext ctx,
    TargetResult target)
  {
    if (!target.IsValid)
      return ValidationResult.Fail("Invalid target");

    var item = ctx.ItemInstance;
    
    if (item?.Data?.Skill.SkillId != "LineAttackSkill")
      return ValidationResult.Fail("Item is not AreaCircle skill");

    if (float.IsNaN(target.Origin.x) || float.IsNaN(target.Origin.y))
      return ValidationResult.Fail("Invalid area center");

    return ValidationResult.Success();
  }
}