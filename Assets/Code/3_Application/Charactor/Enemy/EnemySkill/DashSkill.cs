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

  private float _prepareTime = 0.25f;
  private float _hitRadius = 0.4f;

  private float _nextReadyTime;

  private LayerMask _targetMask;

  public DashSkill(
      float dashSpeed,
      float duration,
      float damage,
      float cooldown,
      float minRange,
      float maxRange,
      LayerMask mask)
  {
    _speed = dashSpeed;
    _duration = duration;
    _damage = damage;
    Cooldown = cooldown;

    MinRange = minRange;
    MaxRange = maxRange;

    _targetMask = mask;
  }

  public void Initialize(Transform owner, EnemyCombat combat)
  {
    _owner = owner;
    _combat = combat;
  }

  public void StartUse(Vector2 direction)
  {
    _nextReadyTime = Time.time + Cooldown;

    if (direction.sqrMagnitude > 0.01f)
      direction.Normalize();

    _combat.StartCoroutine(DashRoutine(direction));
  }

  private IEnumerator DashRoutine(Vector2 dir)
  {
    yield return PreparePhase();

    yield return DashPhase(dir);

    EndPhase();
  }

  private IEnumerator PreparePhase()
  {
    _combat.OnRequestStopMovement?.Invoke(_prepareTime);
    _combat.OnPlayPrepareDash?.Invoke();

    yield return new WaitForSeconds(_prepareTime);
  }

  private IEnumerator DashPhase(Vector2 dir)
  {
    HashSet<Collider2D> hitAlready = new();

    Vector2 impulse = dir * _speed;

    _combat.OnRequestDash?.Invoke(impulse, _duration);
    _combat.OnPlayDash?.Invoke();

    float end = Time.time + _duration;

    while (Time.time < end)
    {
      TryDamage(hitAlready);
      yield return null;
    }
  }

  private void EndPhase()
  {
    _combat.OnPlayEndDash?.Invoke();
  }

  private void TryDamage(HashSet<Collider2D> hitAlready)
  {
    Collider2D hit = Physics2D.OverlapCircle(_owner.position, _hitRadius, _targetMask);

    if (hit == null || hitAlready.Contains(hit))
      return;

    hitAlready.Add(hit);

    if (hit.TryGetComponent<IDamageable>(out var dmg))
    {
      dmg.TakeDamage(_damage, Vector2.zero, 0, 0);
      _combat.OnPlayHit?.Invoke();
    }
  }

  public void DrawGizmos()
  {
    if (_owner == null)
      return;

    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(_owner.position, _hitRadius);
  }
}