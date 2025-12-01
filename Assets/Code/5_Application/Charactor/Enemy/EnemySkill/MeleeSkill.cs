using System.Collections;
using UnityEngine;
public class MeleeSkill : IEnemySkill
{
    public float Range { get; private set; }
    public float Cooldown { get; private set; }
    public bool IsReady => Time.time >= _nextReadyTime;

    private float _damage;
    private Transform _owner;
    private LayerMask _targetMask;
    private EnemyCombat _combat;

    // windup/recovery durations (A = stop before attack)
    private float _windup = 0.25f;
    private float _recovery = 0.45f;
    private float _nextReadyTime;

    public MeleeSkill(float range, float damage, float cooldown, LayerMask mask, float windup = 0.25f, float recovery = 0.45f)
    {
        Range = range;
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
        _combat.StartCoroutine(DoAttack(direction));
    }

    private IEnumerator DoAttack(Vector2 dir)
    {
        _nextReadyTime = Time.time + Cooldown;

        _combat.OnRequestStopMovement?.Invoke(_windup + _recovery);

        _combat.OnPlayAttack?.Invoke("melee");
        yield return new WaitForSeconds(_windup);

        Vector2 origin = _owner.position;
        Vector2 center = origin + dir.normalized * Range;
        float radius = 0.5f;
        Collider2D hit = Physics2D.OverlapCircle(center, radius, _targetMask);
        if (hit != null && hit.TryGetComponent<IDamageable>(out var dmg))
        {
            dmg.TakeDamage((int)_damage);
            _combat.OnPlayHit?.Invoke();
        }

        yield return new WaitForSeconds(_recovery);
    }
}