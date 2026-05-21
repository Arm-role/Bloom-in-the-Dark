using System;
using UnityEngine;

public class BeamExecutor :
  MonoBehaviour,
  ISkillExecutor,
  IPoolable<GameObject>,
  IDestructible
{
  private LineMeleeSkill _skill;
  private GameObject _owner;
  private InteractionIntent _intent;
  private Vector2 _origin;

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
    if (payload is not BeamPayload beamPayload)
      return false;

    if (!beamPayload.IsValid)
      return false;

    // ทิศ beam ล็อกตอน cast — ลำแสงไม่หมุนตามเมาส์ระหว่าง duration
    _skill = new LineMeleeSkill(
      beamPayload.DamagePerTick,
      beamPayload.Range,
      beamPayload.Width,
      beamPayload.KnockForce,
      beamPayload.KnockDuration,
      direction.normalized
    );

    _origin = origin;
    _owner = owner;
    _intent = intent;

    _duration = beamPayload.Duration;
    _tickInterval = beamPayload.TickInterval;
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
      _skill.Cast(_owner, _intent, _origin);
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
