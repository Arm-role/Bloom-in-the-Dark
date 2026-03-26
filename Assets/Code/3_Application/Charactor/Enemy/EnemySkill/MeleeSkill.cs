using System.Collections;
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
    if (_target == null) return;

    // Set initial position for movement check
    _lastTargetPos = _target.position;

    bool targetIsMoving = IsTargetMoving();
    _combat.OnRequestHoldPosition?.Invoke(!targetIsMoving);

    _combat.StartCoroutine(DoAttack(direction));
  }

  private IEnumerator DoAttack(Vector2 dir)
  {
    _nextReadyTime = Time.time + Cooldown;

    _combat.OnRequestStopMovement?.Invoke(_windup + _recovery);

    _combat.OnPlayAttack?.Invoke("melee");
    yield return new WaitForSeconds(_windup);

    // Hit detection
    Vector2 origin = _owner.position;
    Vector2 center = origin + dir.normalized * Range;
    float radius = 0.5f;

    Collider2D hit = Physics2D.OverlapCircle(center, radius, _targetMask);
    if (hit != null && hit.TryGetComponent<IDamageable>(out var dmg))
    {
      var ctx = new DamageContext(
        source: _owner.gameObject,
        intent: InteractionIntent.None,
        damage: _damage,
        direction: Vector2.zero,
        force: 0,
        dration: 0
      );

      dmg.TakeDamage(ctx);
      _combat.OnPlayHit?.Invoke();
    }

    yield return new WaitForSeconds(_recovery);

    // release hold so controller can walk again
    _combat.OnRequestHoldPosition?.Invoke(false);
  }

  private bool IsTargetMoving()
  {
    float dist = Vector2.Distance(_lastTargetPos, _target.position);
    _lastTargetPos = _target.position;
    return dist > 0.01f;
  }
}
