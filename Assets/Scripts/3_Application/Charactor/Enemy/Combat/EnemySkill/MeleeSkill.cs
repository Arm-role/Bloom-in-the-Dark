using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeSkill : IEnemySkill
{
  public float Range { get; private set; }
  public float Cooldown { get; private set; }
  public bool IsReady => Time.time >= _nextReadyTime;

  public float MinRange => 0f;
  public float MaxRange => Range;
  public int Priority => 100;
  public float Weight => 1f;

  private float _damage;
  private float _windup;
  private float _recovery;
  private float _nextReadyTime;

  private Transform _owner;
  private Transform _target;
  private EnemyCombat _combat;

  private LayerMask _targetMask;
  private Vector2 _lastTargetPos;

  private bool _isExecuting = false;
  public bool IsExecuting => _isExecuting;
  public MeleeSkill(float range, float damage, float cooldown, LayerMask mask,
                    float windup = 0.25f, float recovery = 0.45f)
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
    _target = _combat.Target;
    if (_target == null)
      return;

    // Set initial position for movement check
    _lastTargetPos = _target.position;

    _combat.OnRequestHoldPosition?.Invoke(true);
    _combat.OnNavigationPauseRequested?.Invoke(true);

    _combat.StartCoroutine(DoAttack(direction));
  }

  private IEnumerator DoAttack(Vector2 dir)
  {
    _isExecuting = true;

    _nextReadyTime = Time.time + Cooldown;
    _combat.OnRequestStopMovement?.Invoke(_windup + _recovery);
    _combat.OnPlayAttack?.Invoke("melee");

    bool impactHandled = false;
    void OnImpact()
    {
      if (impactHandled) return;
      impactHandled = true;
      TryHitTarget(dir);
    }

    _combat.OnAnimationImpact += OnImpact;

    yield return new WaitForSeconds(_windup);

    // fallback ถ้า event ไม่ยิงภายใน windup
    if (!impactHandled)
    {
      impactHandled = true;   // ป้องกัน event ที่ยิงช้าทำ damage ซ้ำ
      TryHitTarget(dir);
    }

    yield return new WaitForSeconds(_recovery);

    _combat.OnAnimationImpact -= OnImpact;  // unsubscribe หลัง recovery จบ

    _combat.OnRequestHoldPosition?.Invoke(false);
    _combat.OnNavigationPauseRequested?.Invoke(false);

    _isExecuting = false;
  }

  private void TryHitTarget(Vector2 dir)
  {
    if (_target == null) return;

    float ownerRadius = _owner.GetComponent<ICombatEntity>()?.CombatRadius ?? 0.5f;
    float targetRadius = _target.GetComponent<ICombatEntity>()?.CombatRadius ?? 0.5f;
    float attackPadding = 0.15f;

    // edge distance = center dist - ownerRadius - targetRadius
    float edgeDist = CombatDistanceUtility.EdgeDistance(
        _owner, ownerRadius,
        _target, targetRadius);

    // ✅ เทียบ edge distance กับ Range ตรงๆ — ไม่บวก radius ซ้ำ
    if (edgeDist > Range + attackPadding)
      return;

    // OverlapCircle ใช้ center-to-center เต็มๆ
    float centerHitRange = Range + ownerRadius + targetRadius + attackPadding;

    Collider2D[] hits = Physics2D.OverlapCircleAll(
        _owner.position,
        centerHitRange,
        _targetMask);

    HashSet<Transform> hitTargets = new();

    foreach (Collider2D hit in hits)
    {
      var dmg = hit.GetComponentInParent<IDamageable>();
      if (dmg == null) continue;

      var damageableTransform = ((Component)dmg).transform;
      if (!hitTargets.Add(damageableTransform)) continue;

      var ctx = new DamageContext(
          source: _owner.transform,
          intent: InteractionIntent.None,
          damage: Mathf.RoundToInt(_damage),
          direction: dir,
          force: 0,
          dration: 0);

      if (dmg.TakeDamage(ctx))
        _combat.OnTargetDeath?.Invoke(damageableTransform);

      _combat.OnPlayHit?.Invoke();
    }
  }
}
