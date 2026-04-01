using System;
using UnityEngine;

public interface IDamageable
{
  event Action<CharacterDamageResult> OnDamaged;
  void TakeDamage(DamageContext damageContext);
}
public readonly struct CharacterDamageResult
{
  public readonly int Damage;
  public readonly Vector3 Hitbox;
  public readonly Vector2 Direction;
  public readonly bool IsDead;

  public CharacterDamageResult(
      int damage,
      Vector3 hitbox,
      Vector2 direction,
      bool isDead)
  {
    Damage = damage;
    Hitbox = hitbox;
    Direction = direction;
    IsDead = isDead;
  }
}

public struct DamageContext
{
  public readonly GameObject Source;
  public readonly InteractionIntent Intent;

  public readonly int Damage;
  public readonly Vector2 HitDirection;
  public readonly float KnockForce;
  public readonly float KnockDration;

  public DamageContext(
    GameObject source,
    InteractionIntent intent,
    int damage,
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