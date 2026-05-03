using System;
using UnityEngine;

public interface IHealthable
{
  event Action<PlayerHealthResult> OnHeal;
   void Heal(HealthContext energyContext);
}
public readonly struct PlayerHealthResult
{
  public readonly int Amount;
  public readonly Vector3 Hitbox;

  public PlayerHealthResult(
      int amount,
      Vector3 hitbox)
  {
    Amount = amount;
    Hitbox = hitbox;
  }
}

public struct HealthContext
{
  public readonly int Amount;

  public HealthContext(
    int ammount)
  {
    Amount = ammount;
  }
}