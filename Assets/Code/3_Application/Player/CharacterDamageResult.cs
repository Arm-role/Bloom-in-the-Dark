using UnityEngine;

public readonly struct CharacterDamageResult
{
  public readonly float Damage;
  public readonly Vector2 Direction;
  public readonly bool IsDead;

  public CharacterDamageResult(
      float damage,
      Vector2 direction,
      bool isDead)
  {
    Damage = damage;
    Direction = direction;
    IsDead = isDead;
  }
}