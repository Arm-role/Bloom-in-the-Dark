using UnityEngine;

public class LineMeleeSkill : ISkill
{
    private readonly ToolItemInstance _tool;
    private readonly InteractionHandleContext _context;

    public LineMeleeSkill(ToolItemInstance weapon, InteractionHandleContext context)
    {
        _tool = weapon;
        _context = context;
    }

    public void Cast(Vector2 playerPos)
    {
        float range = _tool.Range;       // ระยะโจมตีไปข้างหน้า
        float width = 2;       // ความกว้างของโจมตี
        float damage = _tool.Damage;
        float knock = _tool.KnockForce;
        float knockTime = _tool.KnockDuration;

        Vector2 dir = _context.PlayerDirection.Value;     // ทิศที่ผู้เล่นกำลังหัน

        // จุดกลางของ hitbox
        Vector2 center = playerPos + dir * (range * 0.5f);

        // ขนาดของ boxcast
        Vector2 size = new Vector2(range, width);

        // หาเป้าหมาย
        Collider2D[] hits = Physics2D.OverlapBoxAll(
            center,
            size,
            Vector2.SignedAngle(Vector2.right, dir),
            LayerMask.GetMask("Enemy")
        );

        Debug.Log("Cast");

        foreach (var hit in hits)
        {
            Vector2 enemyPos = hit.transform.position;

            // knockback
            if (hit.TryGetComponent<KnockbackSimulator>(out var knockSim))
            {
                knockSim.ApplyKnockback(dir, knock, knockTime);
            }

            Debug.Log("Found Enemy");

            // damage
            if (hit.TryGetComponent<IDamageable>(out var dmgable))
            {
                dmgable.TakeDamage(damage);
            }
        }
    }
}
