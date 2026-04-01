using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "EnemyPattern/DashAttack")]
public class DashAttackPattern : EnemyPattern
{
  public float windupTime;
  public float afterAttackWait;

  public override IEnumerator Run(EnemyController enemy, Transform target)
  {
    var combat = enemy.Combat;
    var dash = combat.GetSkill<DashSkill>();

    if (dash == null)
      yield break;

    float approachDistance = dash.MaxRange - 0.5f;

    while (enemy.Health.IsAlive && target != null)
    {
      yield return ApproachTarget(enemy, target, approachDistance);

      yield return Windup(enemy);

      yield return ExecuteAttack(enemy, target);

      yield return Recovery();
    }
  }

  private IEnumerator ApproachTarget(EnemyController enemy, Transform target, float distance)
  {
    while (Vector2.Distance(enemy.transform.position, target.position) > distance)
    {
      if (!enemy.Health.IsAlive)
        yield break;

      yield return null;
    }
  }

  private IEnumerator Windup(EnemyController enemy)
  {
    enemy.Combat.OnRequestStopMovement?.Invoke(windupTime);

    float end = Time.time + windupTime;

    while (Time.time < end)
    {
      if (!enemy.Health.IsAlive)
        yield break;

      yield return null;
    }
  }

  private IEnumerator ExecuteAttack(EnemyController enemy, Transform target)
  {
    var combat = enemy.Combat;

    float dist = Vector2.Distance(enemy.transform.position, target.position);

    var skill = combat.SelectSkill(dist);

    if (skill != null)
    {
      Vector2 dir = (target.position - enemy.transform.position).normalized;
      combat.UseSkill(skill, dir);
    }

    yield return null;
  }

  private IEnumerator Recovery()
  {
    yield return new WaitForSeconds(afterAttackWait);
  }
}
