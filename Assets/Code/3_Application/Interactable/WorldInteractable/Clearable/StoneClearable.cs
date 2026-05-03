public class StoneClearable : ClearableState
{
  public override ETargetType TargetType 
    => ETargetType.Interactable;
  public override EInteractionIntentType RequiredIntent
    => EInteractionIntentType.Dig;
  
  public override bool CanBeClearedBy(IItemInstance item)
  {
    return item.Data.HasTag(TagLibrary.Get("Tool.Pickaxe"));
  }
}