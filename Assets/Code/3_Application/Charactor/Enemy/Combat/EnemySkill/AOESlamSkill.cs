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
  private LayerMask _targetMask;

  private float _windupTime;
  private float _riseDuration;
  private float _fallDuration;
  private float _recoveryTime;

  private float _nextReadyTime;

  public AOESlamSkill(
      float minRange,
      float maxRange,
      float hitRadius,
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
    float totalStop = _windupTime + _riseDuration + _fallDuration + _recoveryTime;
    _combat.OnRequestStopMovement?.Invoke(totalStop);
    _combat.OnNavigationPauseRequested?.Invoke(true);

    // --- Phase 1: Windup ---
    _combat.OnPlaySlamWindup?.Invoke();
    yield return new WaitForSeconds(_windupTime);

    if (!IsOwnerAlive()) { Cleanup(); yield break; }

    // --- Phase 2: Rise — ช้าๆ ก่อน ---
    var target = _combat.Target;
    Vector2 destination = target != null ? (Vector2)target.position : (Vector2)_owner.position;

    _combat.OnRequestDisableCollision?.Invoke();
    _combat.OnPlaySlamRise?.Invoke();

    // Rise ใช้ velocity ต่ำ — เคลื่อนที่แค่ครึ่งทาง
    Vector2 midPoint = Vector2.Lerp((Vector2)_owner.position, destination, 0.5f);
    Vector2 riseImpulse = (midPoint - (Vector2)_owner.position) / _riseDuration;
    _combat.OnRequestSlam?.Invoke(riseImpulse, _riseDuration);

    yield return new WaitForSeconds(_riseDuration);

    if (!IsOwnerAlive()) { Cleanup(); yield break; }

    // --- Phase 3: Fall — พุ่งเร็วลงหา target ---
    _combat.OnPlaySlamFall?.Invoke();

    // Fall ใช้ velocity สูง — พุ่งจาก midPoint ถึง destination
    Vector2 fallImpulse = (destination - (Vector2)_owner.position) / _fallDuration;
    _combat.OnRequestSlam?.Invoke(fallImpulse, _fallDuration);

    yield return new WaitForSeconds(_fallDuration);

    if (!IsOwnerAlive()) { Cleanup(); yield break; }

    // --- Phase 4: Land ---
    _combat.OnRequestEnableCollision?.Invoke();
    _combat.OnPlaySlamLand?.Invoke();

    ApplyDamage();
    _combat.OnPlayHit?.Invoke();

    // --- Phase 5: Recovery ---
    yield return new WaitForSeconds(_recoveryTime);

    _combat.OnPlaySlamRecovery?.Invoke();
    _combat.OnNavigationPauseRequested?.Invoke(false);
  }

  private void Cleanup()
  {
    _combat.OnRequestEnableCollision?.Invoke();
    _combat.OnNavigationPauseRequested?.Invoke(false);
  }

  private void ApplyDamage()
  {
    Collider2D[] hits = Physics2D.OverlapCircleAll(_owner.position, _hitRadius, _targetMask);
    HashSet<Transform> hitTargets = new();

    foreach (var h in hits)
    {
      Transform root = h.transform.root;
      if (!hitTargets.Add(root)) continue;
      if (!h.TryGetComponent<IDamageable>(out var dmg)) continue;

      Vector2 dir = ((Vector2)(h.transform.position - _owner.position)).normalized;

      var ctx = new DamageContext(
          source: _owner,
          intent: InteractionIntent.None,
          damage: Mathf.RoundToInt(_damage),
          direction: dir,
          force: 0,
          dration: 0);

      if (dmg.TakeDamage(ctx))
        _combat.OnTargetDeath?.Invoke(h.transform);
    }
  }

  private bool IsOwnerAlive() => _health?.IsAlive == true;
}