// AOESlamSkill.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOESlamSkill : IEnemySkill
{
  public float Cooldown { get; private set; }
  public bool IsReady => Time.time >= _nextReadyTime;

  public float MinRange { get; private set; }
  public float MaxRange { get; private set; }
  public int Priority => 80;
  public float Weight => 1f;

  private Transform _owner;
  private EnemyCombat _combat;
  private EnemyHealth _health;

  private float _damage;
  private float _hitRadius;
  private float _arcHeight;
  private LayerMask _targetMask;

  private float _windupTime;
  private float _riseDuration;
  private float _fallDuration;
  private float _recoveryTime;

  private float _nextReadyTime;

  private bool _isExecuting = false;
  public bool IsExecuting => _isExecuting;
  public AOESlamSkill(
      float minRange,
      float maxRange,
      float hitRadius,
      float arcHeight,
      float damage,
      float cooldown,
      LayerMask mask,
      float windupTime = 0.45f,
      float riseDuration = 0.4f,
      float fallDuration = 0.2f,
      float recoveryTime = 0.5f)
  {
    MinRange = minRange;
    MaxRange = maxRange;
    _hitRadius = hitRadius;
    _arcHeight = arcHeight;
    _damage = damage;
    Cooldown = cooldown;
    _targetMask = mask;
    _windupTime = windupTime;
    _riseDuration = riseDuration;
    _fallDuration = fallDuration;
    _recoveryTime = recoveryTime;
  }

  public void Initialize(Transform owner, EnemyCombat combat)
  {
    _owner = owner;
    _combat = combat;
    _health = owner.GetComponent<EnemyController>()?.Health;
  }

  public void StartUse(Vector2 direction)
  {
    _nextReadyTime = Time.time + Cooldown;
    _combat.StartCoroutine(DoSlam(direction));
  }

  private IEnumerator DoSlam(Vector2 dir)
  {
    _isExecuting = true;

    float totalStop = _windupTime + _riseDuration + _fallDuration + _recoveryTime;
    _combat.OnRequestStopMovement?.Invoke(totalStop);
    _combat.OnNavigationPauseRequested?.Invoke(true);
    _combat.OnRequestDisablePhysics?.Invoke();
    // --- Phase 1: Windup ---
    _combat.OnPlaySlamWindup?.Invoke();
    yield return new WaitForSeconds(_windupTime);

    if (!IsOwnerAlive()) { Cleanup(); yield break; }

    // --- Phase 2 + 3: Rise + Fall — quadratic bezier arc ---
    var target = _combat.Target;
    Vector2 start = _owner.position;
    Vector2 destination = target != null ? (Vector2)target.position : start;

    // control point อยู่กึ่งกลาง offset ออกทาง perpendicular
    Vector2 toTarget = destination - start;
    Vector2 mid = start + toTarget * 0.5f;
    Vector2 controlPoint = mid + Vector2.up * _arcHeight;

    _combat.OnRequestDisableCollision?.Invoke();
    _combat.OnPlaySlamRise?.Invoke();

    float jumpDuration = _riseDuration + _fallDuration;
    float elapsed = 0f;
    bool switchedToFall = false;

    while (elapsed < jumpDuration)
    {
      if (!IsOwnerAlive()) { Cleanup(); yield break; }

      elapsed += Time.deltaTime;
      float t = Mathf.Clamp01(elapsed / jumpDuration);

      // EaseIn ช่วง fall — ช้าตอนออก เร่งตอนลง
      float curved = t < 0.5f
          ? Mathf.Sin(t * Mathf.PI)              // rise — easeOut
          : t;                                    // fall — linear เร็วขึ้น

      // quadratic bezier: B(t) = (1-t)²P0 + 2(1-t)tP1 + t²P2
      Vector2 pos = Bezier(start, controlPoint, destination, t);
      _owner.position = new Vector3(pos.x, pos.y, _owner.position.z);

      if (!switchedToFall && elapsed >= _riseDuration)
      {
        switchedToFall = true;
        _combat.OnPlaySlamFall?.Invoke();
      }


      yield return null;
    }

    _owner.position = new Vector3(destination.x, destination.y, _owner.position.z);

    if (!IsOwnerAlive()) { Cleanup(); yield break; }

    // --- Phase 4: Land ---
    _combat.OnRequestEnableCollision?.Invoke();
    _combat.OnRequestEnablePhysics?.Invoke();
    _combat.OnPlaySlamLand?.Invoke();

    ApplyDamage();
    _combat.OnPlayHit?.Invoke();

    // --- Phase 5: Recovery ---
    _combat.OnPlaySlamRecovery?.Invoke();
    yield return new WaitForSeconds(_recoveryTime);

    _combat.OnNavigationPauseRequested?.Invoke(false);

    _isExecuting = false;
  }

  private Vector2 Bezier(Vector2 p0, Vector2 p1, Vector2 p2, float t)
  {
    float u = 1f - t;
    return u * u * p0 + 2f * u * t * p1 + t * t * p2;
  }

  private void Cleanup()
  {
    _isExecuting = false;
    _combat.OnRequestEnableCollision?.Invoke();
    _combat.OnRequestEnablePhysics?.Invoke();
    _combat.OnNavigationPauseRequested?.Invoke(false);
  }

  private void ApplyDamage()
  {
    Collider2D[] hits = Physics2D.OverlapCircleAll(_owner.position, _hitRadius, _targetMask);
    HashSet<Transform> hitTargets = new();

    foreach (var h in hits)
    {
      var dmg = h.GetComponentInParent<IDamageable>();
      if (dmg == null) continue;

      var damageableTransform = ((Component)dmg).transform;
      if (!hitTargets.Add(damageableTransform)) continue;

      Vector2 dir = ((Vector2)(damageableTransform.position - _owner.position)).normalized;

      var ctx = new DamageContext(
          source: _owner,
          intent: InteractionIntent.None,
          damage: Mathf.RoundToInt(_damage),
          direction: dir,
          force: 0,
          dration: 0);

      if (dmg.TakeDamage(ctx))
        _combat.OnTargetDeath?.Invoke(damageableTransform);
    }
  }

  private bool IsOwnerAlive() => _health?.IsAlive == true;
}