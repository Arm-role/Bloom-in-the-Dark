using UnityEngine;

public class PlantExplosionSkill : ISkill
{
    private readonly PlantItemInstance _plant;
    private readonly float _yScale;

    public PlantExplosionSkill(
        PlantItemInstance plant,
        float yScale)
    {
        _plant = plant;
        _yScale = yScale;
    }

    public void Cast(Vector2 pos)
    {
        float radius = _plant.AreaRadius;
        float dmg = _plant.Damage;

        Collider2D[] hits = Physics2D.OverlapCircleAll(pos, radius, LayerMask.GetMask("Enemy"));

        foreach (var hit in hits)
        {
            Vector2 enemyPos = hit.transform.position;

            if (!IsInsideEllipse(enemyPos, pos, radius))
                continue;

            // Knockback
            if (hit.TryGetComponent<KnockbackSimulator>(out var knock))
            {
                Vector2 dir = (enemyPos - pos).normalized;
                dir.y *= _yScale;

                float dist = Vector2.Distance(enemyPos, pos);
                float t = 1f - (dist / radius);

                knock.ApplyKnockback(dir, _plant.KnockForce * t, _plant.KnockDuration);
            }

            // Damage
            if (hit.TryGetComponent<IDamageable>(out var dmgable))
                dmgable.TakeDamage(dmg);
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
