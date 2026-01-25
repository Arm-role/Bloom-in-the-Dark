using UnityEngine;

public class LineMeleeSkill : ISkill
{
    public float Range { get; set; } // ระยะโจมตีไปข้างหน้า
    public float Width { get; set; } // ความกว้างของโจมตี
    public float Damage { get; set; }
    public float KnockForce { get; set; }
    public float KnockDoraction { get; set; }

    public Vector2 Direction { get; set; }
    public Vector2 Size => new(Range, Width);
    public float Angle => Vector2.SignedAngle(Vector2.right, Direction);

    public void Cast(Vector2 pos)
    {
        Vector2 dir = Direction.normalized;

        // center ของ box (เริ่มจากตัวละคร → ดันไปครึ่ง range)
        Vector2 center = pos + dir * (Range * 0.5f);

        Vector2 size = new(Range, Width);

        Collider2D[] hits = Physics2D.OverlapBoxAll(
            center,
            size,
            Angle,
            LayerMask.GetMask("Enemy")
        );

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<IDamageable>(out var dmgable))
            {
                dmgable.TakeDamage(
                    Damage,
                    dir,
                    KnockForce,
                    KnockDoraction);
            }
        }
    }
}