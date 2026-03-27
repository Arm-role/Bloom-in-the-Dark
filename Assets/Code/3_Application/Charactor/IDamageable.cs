using System;
using UnityEngine;

public interface IDamageable
{
  event Action<CharacterDamageResult> OnDamaged;
  void TakeDamage(DamageContext damageContext);
}

public struct DamageContext
{
  public readonly GameObject Source;
  public readonly InteractionIntent Intent;

  public readonly float Damage;
  public readonly Vector2 HitDirection;
  public readonly float KnockForce;
  public readonly float KnockDration;

  public DamageContext(
    GameObject source, 
    InteractionIntent intent,
    float damage, 
    Vector2 direction,
    float force, 
    float dration)
  {
    Source = source;
    Intent = intent;
    Damage = damage;
    HitDirection = direction;
    KnockForce = force;
    KnockDration = dration;
  }
}