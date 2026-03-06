using System;

[Serializable]
public class IntentCostEntry
{
  public InteractionIntentMatchRule MatchRule;

  public int EnergyCost;
  public int ItemCost;
  public float Cooldown;
}