using UnityEngine;

public class AreaCircleSkill : ISkill
{
  private readonly float _yScale;
  private readonly float _damage;
  private readonly float _radius;
  private readonly float _knokForce;
  private readonly float _knokDuration;

  public AreaCircleSkill(float yScale, float damage, float radius, float knokForce, float knokDuration)
  {
    _yScale = yScale;
    _damage = damage;
    _radius = radius;
    _knokForce = knokForce;
    _knokDuration = knokDuration;
  }

  public void Cast(Vector2 pos)
  {
    Collider2D[] hits = Physics2D.OverlapCircleAll(pos, _radius, LayerMask.GetMask("Enemy"));

    foreach (var hit in hits)
    {
      Vector2 enemyPos = hit.transform.position;

      if (!IsInsideEllipse(enemyPos, pos, _radius))
        continue;

      // Damage
      if (hit.TryGetComponent<IDamageable>(out var dmgable))
      {
        Vector2 dir = (enemyPos - pos).normalized;
        dir.y *= _yScale;

        float dist = Vector2.Distance(enemyPos, pos);
        float t = 1f - (dist / _radius);

        dmgable.TakeDamage(
          _damage,
          dir,
          _knokForce,
          _knokDuration);
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