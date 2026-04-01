using System;
using UnityEngine;

public interface IEnergyable
{
  event Action<PlayerEnergyResult> OnEnergy;
  void AddEnergy(EnergyContext energyContext);
}

public readonly struct PlayerEnergyResult
{
  public readonly int Energy;
  public readonly Vector3 Hitbox;

  public PlayerEnergyResult(
      int energy,
      Vector3 hitbox)
  {
    Energy = energy;
    Hitbox = hitbox;
  }
}

public struct EnergyContext
{
  public readonly int Energy;

  public EnergyContext(
    int engergy)
  {
    Energy = engergy;
  }
}