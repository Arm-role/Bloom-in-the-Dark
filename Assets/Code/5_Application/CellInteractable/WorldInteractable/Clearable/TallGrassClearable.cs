public class TallGrassClearable : ClearableState
{
  public override string ToolName => "Hoe";
  public override EInteractionIntentType RequiredIntent
    => EInteractionIntentType.Dig;
}