public class TreeClearable : ClearableState
{
  public override string ToolName => "Axe";
  public override EInteractionIntentType RequiredIntent
    => EInteractionIntentType.MeleeAttack;
}