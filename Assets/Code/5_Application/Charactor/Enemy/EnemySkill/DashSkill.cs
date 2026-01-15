using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashSkill : IEnemySkill
{
    public float Cooldown { get; private set; }
    public bool IsReady => Time.time >= _nextReadyTime;

    public float MinRange { get; private set; }
    public float MaxRange { get; private set; }
    public int Priority => 50;
    public float Weight => 1f;
    public float DashDistance => _speed * _duration;

    private Transform _owner;
    private EnemyCombat _combat;

    private float _speed;
    private float _duration;
    private float _damage;
    private float _nextReadyTime;

    private LayerMask _targetMask;

    public DashSkill(float dashSpeed, float duration, float damage,
                  float cooldown, float minRange, float maxRange, LayerMask mask)
    {
        _speed = dashSpeed;
        _duration = duration;
        _damage = damage;
        Cooldown = cooldown;
        _targetMask = mask;

        MinRange = minRange;
        MaxRange = maxRange;
    }

    public void Initialize(Transform owner, EnemyCombat combat)
    {
        _owner = owner;
        _combat = combat;
    }

    public void StartUse(Vector2 direction)
    {
        // เริ่มร่ายสกิล (cooldown)
        _nextReadyTime = Time.deltaTime + Cooldown;

        // ระบุทิศก่อน (ให้ตัวหันไป)
        if (direction.sqrMagnitude > 0.01f)
            direction.Normalize();

        _combat.StartCoroutine(ExecuteDash(direction));
    }

    private IEnumerator ExecuteDash(Vector2 dir)
    {
        HashSet<Collider2D> hitAlready = new HashSet<Collider2D>();

        Vector2 impulse = dir * _speed;
        _combat.OnRequestDash?.Invoke(impulse, _duration);
        _combat.OnPlayDash?.Invoke();

        float end = Time.time + _duration;

        while (Time.time < end)
        {
            Collider2D hit = Physics2D.OverlapCircle(_owner.position, 0.4f, _targetMask);

            if (hit != null && !hitAlready.Contains(hit))
            {
                hitAlready.Add(hit); // ทำเครื่องหมายว่าตีแล้ว

                if (hit.TryGetComponent<IDamageable>(out var dmg))
                {
                    dmg.TakeDamage(_damage);
                    _combat.OnPlayHit?.Invoke();
                }
            }

            yield return null;
        }
    }
}