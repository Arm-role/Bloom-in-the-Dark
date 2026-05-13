public class GridTargetValidator : ITargetValidator
{
  public ValidationResult Validate(
    InteractionHandleContext ctx,
    TargetResult target)
  {
    if (!target.IsValid)
      return ValidationResult.Fail("No target cells");

    var tagRequest = TagLibrary.Get("Tool");

    if (ctx.ItemInstance?.Data?.HasTag(tagRequest) == true)
      return ValidationResult.Fail("Item is not a tool");

    foreach (var cell in target.Cells)
    {
      if (!ValidateCell(cell, out var reason))
        return ValidationResult.Fail(reason);
    }

    return ValidationResult.Success();
  }

  private bool ValidateCell(
    WorldCell cell,
    out string reason)
  {
    reason = null;

    if (cell == null)
    {
      reason = "Invalid cell";
      return false;
    }

    if (!cell.CanInteract)
    {
      reason = "Nothing to interact with";
      return false;
    }

    return true;
  }
}