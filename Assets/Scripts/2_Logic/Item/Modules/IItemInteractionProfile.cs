using System.Collections.Generic;

public interface IItemInteractionProfile
{
  public float Range { get; }
  public float Damage  { get; }
  public IReadOnlyList<EInteractionIntentType> SupportedIntents  { get; }
}