using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearDiveSkill : IEnemySkill
{
  public float Cooldown { get; private set; }
  public bool IsReady => Time.time >= _nextReadyTime;
  public float MinRange { get; private set; }
  public float MaxRange { get; private set; }
  public int Priority => 80;
  public float Weight => 1f;
  public bool IsExecuting => _isExecuting;

  private Transform _owner;
  private EnemyCombat _combat;
  private EnemyHealth _health;

  private float _damage;
  private float _hitRadius;
  private float _knockbackForce;
  private float _riseHeight;
  private LayerMask _targetMask;

  private float _windupTime;
  private float _riseDuration;
  private float _peakDuration;
  private float _fallDuration;
  private float _recoveryTime;
  private float _nextReadyTime;
  private bool _isExecuting;

  public SpearDiveSkill(
      float minRange, float maxRange,
      float riseHeight, float hitRadius,
      float damage, float knockbackForce,
      float cooldown, LayerMask mask,
      float windupTime = 0.3f,
      float riseDuration = 0.5f,
      float peakDuration = 0.4f,
      float fallDuration = 0.15f,
      float recoveryTime = 0.6f)
  {
    MinRange = minRange;
    MaxRange = maxRange;
    _riseHeight = riseHeight;
    _hitRadius = hitRadius;
    _damage = damage;
    _knockbackForce = knockbackForce;
    Cooldown = cooldown;
    _targetMask = mask;
    _windupTime = windupTime;
    _riseDuration = riseDuration;
    _peakDuration = peakDuration;
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
    _combat.StartCoroutine(DoSpearDive());
  }

  private IEnumerator DoSpearDive()
  {
    _isExecuting = true;

    float totalStop = _windupTime + _riseDuration + _peakDuration + _fallDuration + _recoveryTime;
    _combat.OnRequestStopMovement?.Invoke(totalStop);
    _combat.OnNavigationPauseRequested?.Invoke(true);
    _combat.OnRequestDisablePhysics?.Invoke();

    // Phase 1: Windup
    _combat.OnPlaySpearDiveWindup?.Invoke();
    yield return new WaitForSeconds(_windupTime);

    if (!IsOwnerAlive()) { Cleanup(); yield break; }

    // Phase 2: Rise — ขึ้นตรงๆ สูง แล้วเคลื่อนไปเหนือ target
    var target = _combat.Target;
    Vector2 start = _owner.position;
    Vector2 targetPos = target != null ? (Vector2)target.position : start;
    Vector2 peakPos = new Vector2(targetPos.x, targetPos.y + _riseHeight);

    _combat.OnRequestDisableCollision?.Invoke();
    _combat.OnPlaySpearDiveRise?.Invoke();

    float elapsed = 0f;
    while (elapsed < _riseDuration)
    {
      if (!IsOwnerAlive()) { Cleanup(); yield break; }
      elapsed += Time.deltaTime;
      float t = Mathf.Clamp01(elapsed / _riseDuration);
      // ease-out — เร็วตอนออก ช้าตอนถึง peak
      float curved = 1f - (1f - t) * (1f - t);
      Vector2 pos = Vector2.Lerp(start, peakPos, curved);
      _owner.position = new Vector3(pos.x, pos.y, _owner.position.z);
      yield return null;
    }
    _owner.position = new Vector3(peakPos.x, peakPos.y, _owner.position.z);

    if (!IsOwnerAlive()) { Cleanup(); yield break; }

    // Phase 3: Peak — lock target position ตอนนี้ (player ต้อง dodge ก่อนหมดเวลา)
    _combat.OnPlaySpearDivePeak?.Invoke();
    Vector2 landingPos = target != null ? (Vector2)target.position : peakPos;

    yield return new WaitForSeconds(_peakDuration);

    if (!IsOwnerAlive()) { Cleanup(); yield break; }

    // Phase 4: Dive — ดิ่งลงสู่ locked position เร็วมาก
    _combat.OnPlaySpearDiveFall?.Invoke();
    Vector2 diveStart = _owner.position;
    elapsed = 0f;
    while (elapsed < _fallDuration)
    {
      if (!IsOwnerAlive()) { Cleanup(); yield break; }
      elapsed += Time.deltaTime;
      float t = Mathf.Clamp01(elapsed / _fallDuration);
      // ease-in — เร่งตอนดิ่งลง
      float curved = t * t;
      Vector2 pos = Vector2.Lerp(diveStart, landingPos, curved);
      _owner.position = new Vector3(pos.x, pos.y, _owner.position.z);
      yield return null;
    }
    _owner.position = new Vector3(landingPos.x, landingPos.y, _owner.position.z);

    if (!IsOwnerAlive()) { Cleanup(); yield break; }

    // Phase 5: Land + Hit
    _combat.OnRequestEnableCollision?.Invoke();
    _combat.OnRequestEnablePhysics?.Invoke();
    _combat.OnPlaySpearDiveLand?.Invoke();

    ApplyDamage();
    _combat.OnPlayHit?.Invoke();

    // Phase 6: Recovery
    _combat.OnPlaySpearDiveRecovery?.Invoke();
    yield return new WaitForSeconds(_recoveryTime);

    _combat.OnNavigationPauseRequested?.Invoke(false);
    _isExecuting = false;
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
          force: _knockbackForce,
          dration: 0.2f);

      if (dmg.TakeDamage(ctx))
        _combat.OnTargetDeath?.Invoke(damageableTransform);
    }
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
