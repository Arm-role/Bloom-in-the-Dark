using System;
using UnityEngine;

public interface IDamageable
{
  event Action<CharacterDamageResult> OnDamaged;
  void TakeDamage(float damage, Vector2 dir, float force, float duration);
}