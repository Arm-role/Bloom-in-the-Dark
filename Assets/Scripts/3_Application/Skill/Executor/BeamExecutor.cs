using System;
using UnityEngine;

public class BeamExecutor :
  MonoBehaviour,
  ISkillExecutor,
  IPoolable<GameObject>,
  IDestructible
{
  [Header("Visual")]
  [Tooltip("child visual/preview ของ beam — ถูกยกขึ้นเมื่อวิถีเป็นแนวนอน (ชดเชยมุมกล้อง)")]
  [SerializeField] private Transform _visualRoot;
  [Tooltip("ระยะยก Y สูงสุดเมื่อวิถีแนวนอนเต็ม (0 = ไม่ยก)")]
  [SerializeField] private float _horizontalYOffset = 0.5f;

  private Vector3 _visualBaseLocalPos;

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

  private void Awake()
  {
    if (_visualRoot != null)
      _visualBaseLocalPos = _visualRoot.localPosition;
  }

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

    var dir = direction.normalized;

    // ทิศ beam ล็อกตอน cast — ลำแสงไม่หมุนตามเมาส์ระหว่าง duration
    _skill = new LineMeleeSkill(
      beamPayload.DamagePerTick,
      beamPayload.Range,
      beamPayload.Width,
      beamPayload.KnockForce,
      beamPayload.KnockDuration,
      dir
    );

    ApplyVisualOffset(dir);

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

  // ยก child visual ขึ้นใน world Y เมื่อวิถีแนวนอน — ชดเชยมุมกล้อง isometric
  // ทำใน world space → ไม่ขึ้นกับว่า root GO ถูกหมุนตามทิศหรือไม่
  private void ApplyVisualOffset(Vector2 dir)
  {
    if (_visualRoot == null) return;

    float horizontalFactor = Mathf.Abs(dir.x);

    _visualRoot.localPosition = _visualBaseLocalPos;
    _visualRoot.position += new Vector3(0f, _horizontalYOffset * horizontalFactor, 0f);
  }
}
