using System;
using UnityEngine;

public class ConeAttackExecutor :
    MonoBehaviour,
    ISkillExecutor,
    IPoolable<GameObject>,
    IDestructible
{
  public float TriggerTime = 0.15f;

  private ConeMeleeSkill _skill;
  private GameObject _owner;
  private InteractionIntent _intent;
  private Vector2 _origin;

  private float _duration;
  private float _timer;
  private bool _isInitialized;

  public bool IsAlive { get; set; }
  public event Action<GameObject> OnRequestDestruction;

  private enum Phase { WaitingToTrigger, Active }

  private Phase _phase;

  public bool Initialize(
      Vector2 origin,
      Vector2 direction,
      ISkillDataPayload payload,
      GameObject owner,
      InteractionIntent intent)
  {
    if (payload is not ConeAttackPayload conePayload)
      return false;

    if (!conePayload.IsValid)
      return false;

    // สร้าง ConeShape แล้วแชร์ให้ Skill
    var shape = new ConeShape();
    shape.Setup(conePayload.XAngle, conePayload.Range, conePayload.AngleDeg + 30);

    _skill = new ConeMeleeSkill(
        shape,
        conePayload.Damage,
        conePayload.KnockForce,
        conePayload.KnockDuration,
        direction.normalized
    );

    _origin = origin;
    _owner = owner;
    _intent = intent;
    _timer = 0f;
    _isInitialized = true;

    _duration = conePayload.Duration;
    _phase = Phase.WaitingToTrigger;

    return true;
  }

  private void Update()
  {
    if (!_isInitialized) return;

    _timer += Time.deltaTime;

    switch (_phase)
    {
      case Phase.WaitingToTrigger:
        if (_timer >= TriggerTime)
        {
          _skill.Cast(_owner, _intent, _origin);
          _timer = 0f;               
          _phase = Phase.Active;
        }
        break;

      case Phase.Active:
        if (_timer >= _duration)      
        {
          RequestDestruction();
        }
        break;
    }
  }

  public void OnSpawnFromPool(GameObject ob) { }

  public void OnReturnToPool(GameObject ob)
  {
    _isInitialized = false;
    _timer = 0f;
    _duration = 0f;
    _phase = Phase.WaitingToTrigger;
    _skill = null;
  }

  public void RequestDestruction()
  {
    OnRequestDestruction?.Invoke(gameObject);
  }
}