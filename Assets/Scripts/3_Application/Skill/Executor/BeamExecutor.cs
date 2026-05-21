using System;
using UnityEngine;

public class BeamExecutor :
  MonoBehaviour,
  ISkillExecutor,
  IChanneledSkillExecutor,
  IPoolable<GameObject>,
  IDestructible
{
  [Header("Visual")]
  [Tooltip("child visual/preview ของ beam — หมุนตามทิศ + ยกขึ้นเมื่อวิถีแนวนอน (ชดเชยมุมกล้อง)")]
  [SerializeField] private Transform _visualRoot;
  [Tooltip("ระยะยก Y สูงสุดเมื่อวิถีแนวนอนเต็ม (0 = ไม่ยก)")]
  [SerializeField] private float _horizontalYOffset = 0.5f;

  private const string ActionLockKey = "beam";

  private Vector3 _visualBaseLocalPos;

  private LineMeleeSkill _skill;
  private GameObject _owner;
  private InteractionIntent _intent;
  private Vector2 _origin;

  private IPlayerInput _input;
  private IPlayerInteractor _interactor;

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

    // ทิศเริ่มต้น — จะถูก steer ตามเมาส์ทุกเฟรมใน Update
    _skill = new LineMeleeSkill(
      beamPayload.DamagePerTick,
      beamPayload.Range,
      beamPayload.Width,
      beamPayload.KnockForce,
      beamPayload.KnockDuration,
      dir
    );

    _origin = origin;
    _owner = owner;
    _intent = intent;

    _duration = beamPayload.Duration;
    _tickInterval = beamPayload.TickInterval;
    _elapsed = 0f;
    _tickTimer = 0f;
    _isInitial = true;

    UpdateVisual(dir);

    return true;
  }

  // เรียกโดย SkillSpawnController หลัง Initialize — ผูก input/interactor ของ player
  public void BindChannel(IPlayerInput input, IPlayerInteractor interactor)
  {
    _input = input;
    _interactor = interactor;

    // ล็อกการเดินตลอด channel — lock เป็นแบบ timed หมดเวลาเอง (= duration ของ beam)
    _interactor?.TryStartAction(ActionLockKey, _duration);
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

    Steer();

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

  // หมุน beam ตามเมาส์ปัจจุบัน — player ยืนกับที่ระหว่าง channel, beam pivot รอบตัว
  private void Steer()
  {
    if (_input == null || _owner == null)
      return;

    _origin = _owner.transform.position;

    Vector2 aim = (Vector2)_input.PointerWorldPosition - _origin;
    if (aim.sqrMagnitude < 0.0001f)
      return; // เมาส์ทับตัว player — คงทิศเดิม

    Vector2 dir = aim.normalized;
    _skill.Direction = dir;
    UpdateVisual(dir);
  }

  // หมุน + ยก child visual ตามทิศ ; ยกใน world Y เมื่อวิถีแนวนอน (ชดเชยมุมกล้อง isometric)
  private void UpdateVisual(Vector2 dir)
  {
    if (_visualRoot == null)
      return;

    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
    _visualRoot.rotation = Quaternion.Euler(CameraConfig.XAngle, 0f, angle);

    float horizontalFactor = Mathf.Abs(dir.x);
    _visualRoot.localPosition = _visualBaseLocalPos;
    _visualRoot.position += new Vector3(0f, _horizontalYOffset * horizontalFactor, 0f);
  }

  public void RequestDestruction()
  {
    OnRequestDestruction?.Invoke(gameObject);
  }
}
