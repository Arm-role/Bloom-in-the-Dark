using System.Collections;
using UnityEngine;

public class DashSkill : IEnemySkill
{
    public float Range { get; private set; }
    public float Cooldown { get; private set; }
    public bool IsReady => Time.time >= _nextReadyTime;

    private Transform _owner;
    private EnemyCombat _combat;
    private float _speed;
    private float _duration;
    private float _damage;
    private LayerMask _targetMask;
    private float _nextReadyTime;

    public DashSkill(float dashSpeed, float duration, float damage, float cooldown, LayerMask mask)
    {
        _speed = dashSpeed;
        _duration = duration;
        _damage = damage;
        Cooldown = cooldown;
        Range = dashSpeed * duration;
        _targetMask = mask;
    }

    public void Initialize(Transform owner, EnemyCombat combat)
    {
        _owner = owner;
        _combat = combat;
    }

    public void StartUse(Vector2 direction)
    {
        _combat.StartCoroutine(DoDash(direction.normalized));
    }

    private IEnumerator DoDash(Vector2 dir)
    {
        _nextReadyTime = Time.time + Cooldown;

        _combat.OnPlayDash?.Invoke();

        Vector2 impulse = dir * _speed;
        _combat.OnRequestDash?.Invoke(impulse, _duration);

        float endTime = Time.time + _duration;
        while (Time.time < endTime)
        {
            Collider2D hit = Physics2D.OverlapCircle(_owner.position, 0.4f, _targetMask);
            if (hit != null && hit.TryGetComponent<IDamageable>(out var dmg))
            {
                dmg.TakeDamage((int)_damage);
                _combat.OnPlayHit?.Invoke();
            }
            yield return null;
        }
    }
}