using System;
using UnityEngine;

public class AreaCircleExecutor :
  MonoBehaviour,
  ISkillExecutor,
  IPoolable<GameObject>,
  IDestructible
{
  public AreaCircleSkill Skill;
  public float TriggerTime = 0.35f;

  private float timer;

  private bool isInitial;

  public bool IsAlive { get; set; }
  public event Action<GameObject> OnRequestDestruction;

  public bool Initialize(
    Vector2 origin,
    Vector2 direction,
    ISkillDataPayload payload)
  {
    if (payload is not AreaCirclePayload areaCirclePayload)
      return false;

    if (!areaCirclePayload.IsValid)
      return false;
    
    var yScale = Mathf.Cos(areaCirclePayload.XAngle * Mathf.Deg2Rad);
    
    Skill = new AreaCircleSkill(
      yScale,
      areaCirclePayload.Damage,
      areaCirclePayload.Radius,
      areaCirclePayload.KnockForce,
      areaCirclePayload.KnockDuration
    );

    isInitial = true;
    timer = 0;

    Debug.Log("AreaCircleExecutor");
    return true;
  }


  public void OnReturnToPool(GameObject ob)
  {
    isInitial = false;
    timer = 0;
  }

  public void OnSpawnFromPool(GameObject ob)
  {
  }

  void Update()
  {
    if (!isInitial) return;
    timer += Time.deltaTime;
    if (timer >= TriggerTime)
    {
      Skill.Cast(transform.position);
      RequestDestruction();
    }
  }

  public void RequestDestruction()
  {
    OnRequestDestruction?.Invoke(gameObject);
  }
}