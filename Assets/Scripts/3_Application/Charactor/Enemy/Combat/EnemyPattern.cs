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
    Transform cachedTarget = null;
    float cachedRadius = 0.5f;

    while (IsValid(enemy))
    {
      var t = enemy.CurrentTarget;
      if (t != cachedTarget)
      {
        cachedTarget = t;
        cachedRadius = t?.GetComponent<ICombatEntity>()?.CombatRadius ?? 0.5f;
      }

      float d = CombatDistanceUtility.EdgeDistance(
          enemy.transform, enemy.CombatRadius,
          t, cachedRadius);
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

  protected float EdgeDist(EnemyController enemy, Transform target, float targetRadius = -1f)
  {
    if (target == null) return float.MaxValue;
    if (targetRadius < 0f)
      targetRadius = target.GetComponent<ICombatEntity>()?.CombatRadius ?? 0.5f;
    return Mathf.Max(
        CombatDistanceUtility.EdgeDistance(enemy.transform, enemy.CombatRadius, target, targetRadius),
        0f);
  }
}