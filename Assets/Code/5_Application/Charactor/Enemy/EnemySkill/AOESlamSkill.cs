using System.Collections;
using UnityEngine;

public class AOESlamSkill : IEnemySkill
{
    public float Range { get; private set; } // radius
    public float Cooldown { get; private set; }
    public bool IsReady => Time.time >= _nextReadyTime;

    private Transform _owner;
    private EnemyCombat _combat;
    private float _damage;
    private LayerMask _targetMask;
    private float _windup = 0.45f;
    private float _recovery = 0.6f;
    private float _nextReadyTime;

    public AOESlamSkill(float radius, float damage, float cooldown, LayerMask mask, float windup = 0.45f, float recovery = 0.6f)
    {
        Range = radius;
        _damage = damage;
        Cooldown = cooldown;
        _targetMask = mask;
        _windup = windup;
        _recovery = recovery;
    }

    public void Initialize(Transform owner, EnemyCombat combat)
    {
        _owner = owner;
        _combat = combat;
    }

    public void StartUse(Vector2 direction)
    {
        _combat.StartCoroutine(DoSlam());
    }

    private IEnumerator DoSlam()
    {
        _nextReadyTime = Time.time + Cooldown;

        _combat.OnRequestStopMovement?.Invoke(_windup + _recovery);
        _combat.OnPlayAttack?.Invoke("slam");

        yield return new WaitForSeconds(_windup);

        Collider2D[] hits = Physics2D.OverlapCircleAll(_owner.position, Range, _targetMask);
        foreach (var h in hits)
        {
            if (h.TryGetComponent<IDamageable>(out var dmg))
            {
                dmg.TakeDamage((int)_damage);
            }
        }
        _combat.OnPlayHit?.Invoke();

        yield return new WaitForSeconds(_recovery);
    }
}