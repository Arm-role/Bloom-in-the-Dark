public class TreeClearable : ClearableState
{
  public override ETargetType TargetType => ETargetType.Interactable;
  public override EInteractionIntentType RequiredIntent
    => EInteractionIntentType.MeleeAttack;
  
  public override bool CanBeClearedBy(IItemInstance item)
  {
    if (item.Data is not IToolItemData tool)
      return false;

    return tool.ToolType == EToolType.Axe;
  }
}