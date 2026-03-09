
public class TallGrassClearable : ClearableState
{
  public override ETargetType TargetType => ETargetType.Interactable;

  public override EInteractionIntentType RequiredIntent
    => EInteractionIntentType.Dig;

  public override bool CanBeClearedBy(IItemInstance item)
  {
    return item.Data.HasTag(TagLibrary.Get("Tool.Hoe"))||
     item.Data.HasTag(TagLibrary.Get("Tool.Pickaxe"));
  }
}