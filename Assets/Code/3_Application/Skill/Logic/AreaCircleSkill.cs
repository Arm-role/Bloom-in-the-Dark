using System.Collections.Generic;
using UnityEngine;
public class AreaCircleSkill : ISkill
{
  private readonly float _yScale;
  private readonly float _damage;
  private readonly float _radius;
  private readonly float _knockForce;
  private readonly float _knockDuration;

  public AreaCircleSkill(float yScale, float damage, float radius, float knockForce, float knockDuration)
  {
    _yScale = yScale;
    _damage = damage;
    _radius = radius;
    _knockForce = knockForce;
    _knockDuration = knockDuration;
  }

  public void Cast(GameObject owner, InteractionIntent intent, Vector2 pos)
  {
    Collider2D[] hits = Physics2D.OverlapCircleAll(pos, _radius, LayerMask.GetMask("Enemy"));

    HashSet<Transform> hitTargets = new();

    foreach (var hit in hits)
    {
      Transform transform = hit.transform;

      if (hitTargets.Contains(transform))
        continue;

      hitTargets.Add(transform);

      Vector2 enemyPos = transform.position;

      if (!IsInsideEllipse(enemyPos, pos, _radius))
        continue;

      // Damage
      if (hit.TryGetComponent<IDamageable>(out var dmgable))
      {
        Vector2 dir = (enemyPos - pos).normalized;
        dir.y *= _yScale;

        float dist = Vector2.Distance(enemyPos, pos);
        float t = 1f - (dist / _radius);

        int finalDamage = Mathf.RoundToInt(_damage);

        var ctx = new DamageContext(
          source: owner.transform,
          intent: intent,
          damage: finalDamage,
          direction: dir,
          force: _knockForce,
          dration: _knockDuration
        );

        dmgable.TakeDamage(ctx);
      }
    }
  }

  private bool IsInsideEllipse(Vector2 point, Vector2 center, float radius)
  {
    Vector2 local = point - center;
    float x = local.x;
    float y = local.y / _yScale;

    return (x * x + y * y) <= radius * radius;
  }
}