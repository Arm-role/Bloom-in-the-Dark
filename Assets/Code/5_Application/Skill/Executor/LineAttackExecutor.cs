using System;
using UnityEngine;

public class LineAttackExecutor :
  MonoBehaviour,
  ISkillExecutor,
  IPoolable<GameObject>,
  IDestructible
{
  public LineMeleeSkill Skill;
  public float TriggerTime = 0.15f;

  private float timer;
  private bool isInitial;
  private Vector2 _playerPosition;

  public bool IsAlive { get; set; }

  public bool Initialize(
    Vector2 origin,
    Vector2 direction,
    ISkillDataPayload payload)
  {
    if (payload is not LineAttackPayload linePayload)
      return false;

    if (!linePayload.IsValid)
      return false;
    
    Skill = new LineMeleeSkill(
      linePayload.Damage,
      linePayload.Range,
      linePayload.Width,
      linePayload.KnockForce,
      linePayload.KnockDuration,
      direction.normalized);

    _playerPosition = origin;
    isInitial = true;
    timer = 0;
    
    return true;
  }

  public event Action<GameObject> OnRequestDestruction;

  public void RequestDestruction()
  {
    OnRequestDestruction?.Invoke(gameObject);
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
      Skill.Cast(_playerPosition);
      RequestDestruction();
    }
  }
  
  // private void OnDrawGizmos()
  // {
  //   if (Skill == null)
  //     return;
  //
  //   Vector2 origin = transform.position;
  //   Vector2 dir = Skill.Direction.normalized;
  //
  //   Vector2 center = origin + dir * (Skill.Range * 0.5f);
  //
  //   Gizmos.color = Color.red;
  //
  //   Matrix4x4 old = Gizmos.matrix;
  //
  //   Gizmos.matrix = Matrix4x4.TRS(
  //     center,
  //     Quaternion.Euler(0, 0, Skill.Angle),
  //     Vector3.one);
  //
  //   Gizmos.DrawWireCube(Vector3.zero, Skill.Size);
  //   Gizmos.matrix = old;
  //
  //   // ---- visualize origin → attack ----
  //   Gizmos.color = Color.green;
  //   Gizmos.DrawLine(
  //     origin,
  //     origin + dir * Skill.Range
  //   );
  //
  //   Gizmos.DrawSphere(origin, 0.05f);
  // }
  
}