using UnityEngine;

public class ConeMeleeSkill : ISkill
{
    //private readonly WeaponItemInstance _weapon;

    //public ConeMeleeSkill(WeaponItemInstance weapon)
    //{
    //    _weapon = weapon;
    //}

    public void Cast(Vector2 pos)
    {
        //    float range = _weapon.AttackRange;
        //    float angle = _weapon.AttackAngle; // 45°–90°
        //    float damage = _weapon.Damage;

        //    Vector2 dir = _weapon.FaceDirection;

        //    Collider2D[] hits = Physics2D.OverlapCircleAll(
        //        playerPos,
        //        range,
        //        LayerMask.GetMask("Enemy")
        //    );

        //    foreach (var hit in hits)
        //    {
        //        Vector2 toTarget = (Vector2)hit.transform.position - playerPos;

        //        // เช็คว่าศัตรูอยู่ในกรวยหรือไม่
        //        float deltaAngle = Vector2.Angle(dir, toTarget);

        //        if (deltaAngle > angle * 0.5f)
        //            continue;

        //        if (hit.TryGetComponent<IDamageable>(out var dmgable))
        //            dmgable.TakeDamage(damage);
        //    }
    }

  public void Cast(GameObject owner, InteractionIntent intent, Vector2 pos)
  {
    throw new System.NotImplementedException();
  }
}