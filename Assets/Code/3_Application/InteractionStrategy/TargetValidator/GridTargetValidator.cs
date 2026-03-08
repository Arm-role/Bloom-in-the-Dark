public class GridTargetValidator : ITargetValidator
{
  public ValidationResult Validate(
    InteractionHandleContext ctx,
    TargetResult target)
  {
    if (!target.IsValid)
      return ValidationResult.Fail("No target cells");

    if (ctx.ItemInstance?.Data is not IToolItemData)
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

    if (!cell.HasAnyInteractable)
    {
      reason = "Nothing to interact with";
      return false;
    }

    return true;
  }
}