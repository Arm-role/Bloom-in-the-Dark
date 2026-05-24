using System;
using UnityEngine;

public class SkillSelfController
{
  private readonly IEnergyable _energyable;

  public SkillSelfController(IEnergyable energyable)
  {
    _energyable = energyable;
  }

  public void Use(ISkillDataPayload payload, InteractionIntent _)
  {
    if (payload is IncreaseEnergyPayload increaseEnergypayload)
    {
      var finalIncrease = Mathf.RoundToInt(increaseEnergypayload.Increase);

      var context = new EnergyContext(finalIncrease);
      _energyable.AddEnergy(context);
      return;
    }

    throw new ArgumentException(
      $"[SkillSelfController] Unsupported payload type: {payload?.GetType().Name ?? "null"}",
      nameof(payload));
  }
}