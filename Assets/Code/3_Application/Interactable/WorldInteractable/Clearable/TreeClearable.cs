public class TreeClearable : ClearableState
{
  public override ETargetType TargetType 
    => ETargetType.Interactable;
  public override EInteractionIntentType RequiredIntent
    => EInteractionIntentType.Chop;
  
  public override bool CanBeClearedBy(IItemInstance item)
  {
    return item.Data.HasTag(TagLibrary.Get("Tool.Axe"));
  }
}