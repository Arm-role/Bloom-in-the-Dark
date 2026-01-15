using UnityEngine;

public class LineMeleeSkill : ISkill
{
    private readonly InteractionHandleContext _context;

    private readonly float _range;       // ระยะโจมตีไปข้างหน้า
    private readonly float _width;       // ความกว้างของโจมตี
    private readonly float _damage;
    private readonly float _knockForce;
    private readonly float _knockDoraction;
    public LineMeleeSkill(IItemInstance itemInstance, InteractionHandleContext context)
    {
        _range = itemInstance.GetStat(EItemStatType.Range);
        _damage = itemInstance.GetStat(EItemStatType.Damage);
        _knockForce = itemInstance.GetStat(EItemStatType.KnockForce);
        _knockDoraction = itemInstance.GetStat(EItemStatType.KnockDuration);
        _width = 2;
        
        _context = context;
    }

    public void Cast(Vector2 playerPos)
    {
        Vector2 dir = _context.PlayerDirection.Value;     // ทิศที่ผู้เล่นกำลังหัน

        // จุดกลางของ hitbox
        Vector2 center = playerPos + dir * (_range * 0.5f);

        // ขนาดของ boxcast
        Vector2 size = new Vector2(_range, _width);

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
                knockSim.ApplyKnockback(dir, _knockForce, _knockDoraction);
            }

            Debug.Log("Found Enemy");

            // damage
            if (hit.TryGetComponent<IDamageable>(out var dmgable))
            {
                dmgable.TakeDamage(_damage);
            }
        }
    }
}
