using System;
using UnityEngine;

public class PoisonAreaExecutor :
  MonoBehaviour,
  ISkillExecutor,
  IPoolable<GameObject>,
  IDestructible
{
  private AreaCircleSkill _skill;
  private GameObject _owner;
  private InteractionIntent _intent;

  private float _duration;
  private float _tickInterval;
  private float _elapsed;
  private float _tickTimer;

  private bool _isInitial;

  public bool IsAlive { get; set; }
  public event Action<GameObject> OnRequestDestruction;

  public bool Initialize(
    Vector2 origin,
    Vector2 direction,
    ISkillDataPayload payload,
    GameObject owner,
    InteractionIntent intent)
  {
    if (payload is not PoisonAreaPayload poisonPayload)
      return false;

    if (!poisonPayload.IsValid)
      return false;

    var yScale = Mathf.Cos(poisonPayload.XAngle * Mathf.Deg2Rad);

    // พิษไม่ผลัก enemy — knockForce/knockDuration = 0
    _skill = new AreaCircleSkill(
      yScale,
      poisonPayload.DamagePerTick,
      poisonPayload.Radius,
      0f,
      0f
    );

    _owner = owner;
    _intent = intent;

    _duration = poisonPayload.Duration;
    _tickInterval = poisonPayload.TickInterval;
    _elapsed = 0f;
    _tickTimer = 0f;
    _isInitial = true;

    return true;
  }

  public void OnReturnToPool(GameObject ob)
  {
    _isInitial = false;
    _elapsed = 0f;
    _tickTimer = 0f;
  }

  public void OnSpawnFromPool(GameObject ob)
  {
  }

  private void Update()
  {
    if (!_isInitial) return;

    float dt = Time.deltaTime;
    _elapsed += dt;
    _tickTimer += dt;

    // tick แรกเกิดหลังครบ 1 interval — เก็บเศษเวลาด้วย -= กัน tick เพี้ยนตอน framerate ตก
    if (_tickTimer >= _tickInterval)
    {
      _tickTimer -= _tickInterval;
      _skill.Cast(_owner, _intent, transform.position);
    }

    if (_elapsed >= _duration)
    {
      _isInitial = false;
      RequestDestruction();
    }
  }

  public void RequestDestruction()
  {
    OnRequestDestruction?.Invoke(gameObject);
  }
}
