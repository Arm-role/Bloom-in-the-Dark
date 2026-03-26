using UnityEngine;

public class AreaCircleValidator : ITargetValidator
{
  public ValidationResult Validate(
    InteractionHandleContext ctx,
    TargetResult target)
  {
    if (!target.IsValid)
      return ValidationResult.Fail("Invalid target");

    var item = ctx.ItemInstance;

    if (item?.Data?.Skill == null)
      return ValidationResult.Fail("Item is not AreaCircle skill");

    if (target.Extra is not Vector2 center)
      return ValidationResult.Fail("Missing area center");

    if (float.IsNaN(center.x) || float.IsNaN(center.y))
      return ValidationResult.Fail("Invalid area center");

    return ValidationResult.Success();
  }
}