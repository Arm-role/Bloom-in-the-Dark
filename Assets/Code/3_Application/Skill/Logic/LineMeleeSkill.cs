using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class LineMeleeSkill : ISkill
{
  private readonly float _damage;
  private readonly float _range; 
  private readonly float _width;
  private readonly float _knockForce;
  private readonly float _knockDuration;

  public LineMeleeSkill(
    float damage,
    float range,
    float width,
    float knockForce, 
    float knockDuration,
    Vector2 direction)
  {
    _damage = damage;
    _range = range;
    _width = width;
    _knockForce = knockForce;
    _knockDuration = knockDuration;
    Direction = direction;
  }
  
  public Vector2 Direction { get; set; }
  public Vector2 Size => new(_range, _width);
  public float Angle => Vector2.SignedAngle(Vector2.right, Direction);

  public void Cast(GameObject owner, InteractionIntent intent, Vector2 pos)
  {
    Vector2 dir = Direction.normalized;

    Vector2 center = pos + dir * (_range * 0.5f);

    Vector2 size = new(_range, _width);

    Collider2D[] hits = Physics2D.OverlapBoxAll(
      center,
      size,
      Angle,
      LayerMask.GetMask("Enemy")
    );

    HashSet<Transform> hitTargets = new();

    foreach (var hit in hits)
    {
      Transform root = hit.transform.root;

      if (hitTargets.Contains(root))
        continue;

      hitTargets.Add(root);

      if (hit.TryGetComponent<IDamageable>(out var dmgable))
      {
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
}