using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearSlamSkill : IEnemySkill
{
  public float Cooldown { get; private set; }
  public bool IsReady => Time.time >= _nextReadyTime;
  public float MinRange { get; private set; }
  public float MaxRange { get; private set; }
  public int Priority => 70;
  public float Weight => 1f;
  public bool IsExecuting => _isExecuting;

  private Transform _owner;
  private EnemyCombat _combat;
  private EnemyHealth _health;

  private float _damage;
  private float _hitRange;
  private float _hitWidth;
  private float _knockbackForce;
  private float _arcHeight;
  private LayerMask _targetMask;

  private float _windupTime;
  private float _riseDuration;
  private float _fallDuration;
  private float _recoveryTime;
  private float _nextReadyTime;
  private bool _isExecuting;

  public SpearSlamSkill(
      float minRange, float maxRange,
      float hitRange, float hitWidth,
      float arcHeight, float damage,
      float knockbackForce, float cooldown,
      LayerMask mask,
      float windupTime = 0.4f,
      float riseDuration = 0.35f,
      float fallDuration = 0.2f,
      float recoveryTime = 0.5f)
  {
    MinRange = minRange;
    MaxRange = maxRange;
    _hitRange = hitRange;
    _hitWidth = hitWidth;
    _arcHeight = arcHeight;
    _damage = damage;
    _knockbackForce = knockbackForce;
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
    _combat.StartCoroutine(DoSpearSlam(direction));
  }

  private IEnumerator DoSpearSlam(Vector2 dir)
  {
    _isExecuting = true;

    float totalStop = _windupTime + _riseDuration + _fallDuration + _recoveryTime;
    _combat.OnRequestStopMovement?.Invoke(totalStop);
    _combat.OnNavigationPauseRequested?.Invoke(true);
    _combat.OnRequestDisablePhysics?.Invoke();

    // Phase 1: Windup
    _combat.OnPlaySpearSlamWindup?.Invoke();
    yield return new WaitForSeconds(_windupTime);

    if (!IsOwnerAlive()) { Cleanup(); yield break; }

    // Phase 2 + 3: Rise + Fall — bezier arc toward target
    var target = _combat.Target;
    Vector2 start = _owner.position;
    Vector2 destination = target != null ? (Vector2)target.position : start;
    Vector2 slamDir = destination != start ? (destination - start).normalized : dir;

    Vector2 mid = start + (destination - start) * 0.5f;
    Vector2 controlPoint = mid + Vector2.up * _arcHeight;

    _combat.OnRequestDisableCollision?.Invoke();
    _combat.OnPlaySpearSlamRise?.Invoke();

    float jumpDuration = _riseDuration + _fallDuration;
    float elapsed = 0f;
    bool switchedToFall = false;

    while (elapsed < jumpDuration)
    {
      if (!IsOwnerAlive()) { Cleanup(); yield break; }

      elapsed += Time.deltaTime;
      float t = Mathf.Clamp01(elapsed / jumpDuration);
      Vector2 pos = Bezier(start, controlPoint, destination, t);
      _owner.position = new Vector3(pos.x, pos.y, _owner.position.z);

      if (!switchedToFall && elapsed >= _riseDuration)
      {
        switchedToFall = true;
        _combat.OnPlaySpearSlamFall?.Invoke();
      }

      yield return null;
    }

    _owner.position = new Vector3(destination.x, destination.y, _owner.position.z);

    if (!IsOwnerAlive()) { Cleanup(); yield break; }

    // Phase 4: Land + Box Hit
    _combat.OnRequestEnableCollision?.Invoke();
    _combat.OnRequestEnablePhysics?.Invoke();
    _combat.OnPlaySpearSlamLand?.Invoke();

    ApplyDamage(slamDir);
    _combat.OnPlayHit?.Invoke();

    // Phase 5: Recovery
    _combat.OnPlaySpearSlamRecovery?.Invoke();
    yield return new WaitForSeconds(_recoveryTime);

    _combat.OnNavigationPauseRequested?.Invoke(false);
    _isExecuting = false;
  }

  private void ApplyDamage(Vector2 dir)
  {
    // Box ยื่นออกจากตัว boss ไปในทิศที่ฟาด
    Vector2 center = (Vector2)_owner.position + dir * (_hitRange * 0.5f);
    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    Vector2 size = new Vector2(_hitRange, _hitWidth);

    Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, angle, _targetMask);
    HashSet<Transform> hitTargets = new();

    foreach (var h in hits)
    {
      var dmg = h.GetComponentInParent<IDamageable>();
      if (dmg == null) continue;

      var damageableTransform = ((Component)dmg).transform;
      if (!hitTargets.Add(damageableTransform)) continue;

      Vector2 hitDir = ((Vector2)(damageableTransform.position - _owner.position)).normalized;

      var ctx = new DamageContext(
          source: _owner,
          intent: InteractionIntent.None,
          damage: Mathf.RoundToInt(_damage),
          direction: hitDir,
          force: _knockbackForce,
          dration: 0.2f);

      if (dmg.TakeDamage(ctx))
        _combat.OnTargetDeath?.Invoke(damageableTransform);
    }
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

  private bool IsOwnerAlive() => _health?.IsAlive == true;
}
