using System.Collections.Generic;
using UnityEngine;

public class ConeMeleeSkill : ISkill
{
  private readonly ConeShape _shape;
  private readonly float _damage;
  private readonly float _knockForce;
  private readonly float _knockDuration;

  public Vector2 Direction { get; set; }

  public ConeMeleeSkill(
      ConeShape shape,
      float damage,
      float knockForce,
      float knockDuration,
      Vector2 direction)
  {
    _shape = shape;
    _damage = damage;
    _knockForce = knockForce;
    _knockDuration = knockDuration;
    Direction = direction;
  }

  public void Cast(GameObject owner, InteractionIntent intent, Vector2 pos)
  {
    Vector2 forward = Direction.normalized;

    // ใช้ range จาก shape เป็น broad-phase radius
    Collider2D[] hits = Physics2D.OverlapCircleAll(
        pos,
        _shape.Range,
        LayerMask.GetMask("Enemy")
    );

    HashSet<Transform> hitTargets = new();

    foreach (var hit in hits)
    {
      Transform root = hit.transform;
      
      if (hitTargets.Contains(root))
        continue;

      hitTargets.Add(root);

      // ✅ ใช้ position ของ root ให้ตรงกับที่ IsInside() เช็ค
      Vector2 enemyPos = root.position;

      if (!_shape.IsInside(pos, forward, enemyPos))
        continue;

      if (!root.TryGetComponent<IDamageable>(out var dmgable))
        continue;

      Vector2 knockDir = (enemyPos - pos).normalized;

      int finalDamage = Mathf.RoundToInt(_damage);

      var ctx = new DamageContext(
          source: owner.transform,
          intent: intent,
          damage: finalDamage,
          direction: knockDir,
          force: _knockForce,
          dration: _knockDuration
      );

      dmgable.TakeDamage(ctx);
    }
  }
}