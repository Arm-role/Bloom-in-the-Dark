public class StoneClearable : ClearableState
{
  public override string ToolName => "Pickaxe";
  public override EInteractionIntentType RequiredIntent
    => EInteractionIntentType.Dig;
}