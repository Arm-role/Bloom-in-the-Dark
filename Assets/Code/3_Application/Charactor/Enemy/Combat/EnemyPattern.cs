// EnemyPattern.cs — base ไม่มี loop
using System.Collections;
using UnityEngine;

public abstract class EnemyPattern : ScriptableObject
{
  // signature เดิม — child จัดการ loop เอง
  public abstract IEnumerator Run(EnemyController enemy);

  protected bool IsValid(EnemyController enemy)
      => enemy.Health.IsAlive && enemy.CurrentTarget != null;

  protected Transform Target(EnemyController enemy)
      => enemy.CurrentTarget;

  protected IEnumerator ApproachUntil(EnemyController enemy, float edgeDist)
  {
    while (IsValid(enemy))
    {
      float d = CombatDistanceUtility.EdgeDistance(
          enemy.transform, enemy.CombatRadius,
          enemy.CurrentTarget,
          enemy.CurrentTarget.GetComponent<ICombatEntity>()?.CombatRadius ?? 0.5f);
      if (d <= edgeDist) yield break;
      yield return null;
    }
  }

  protected IEnumerator Windup(EnemyController enemy, float duration)
  {
    enemy.Combat.OnRequestStopMovement?.Invoke(duration);
    float end = Time.time + duration;
    while (Time.time < end)
    {
      if (!IsValid(enemy)) yield break;
      yield return null;
    }
  }

  protected IEnumerator Wait(float duration)
  {
    yield return new WaitForSeconds(duration);
  }
}