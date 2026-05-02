// DashAttackPattern.cs — loop อยู่ที่นี่ เหมือนเดิม
using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "EnemyPattern/DashAttack")]
public class DashAttackPattern : EnemyPattern
{
  public float windupTime = 0.3f;
  public float recoveryTime = 0.5f;

  public override IEnumerator Run(EnemyController enemy)
  {
    var dash = enemy.Combat.GetSkill<DashSkill>();
    if (dash == null) yield break;

    while (IsValid(enemy))
    {
      yield return ApproachUntil(enemy, dash.MaxRange - 0.5f);
      if (!IsValid(enemy)) yield break;

      yield return Windup(enemy, windupTime);
      if (!IsValid(enemy)) yield break;

      var target = Target(enemy);
      float dist = CombatDistanceUtility.EdgeDistance(
          enemy.transform, enemy.CombatRadius,
          target, target.GetComponent<ICombatEntity>()?.CombatRadius ?? 0.5f);

      var skill = enemy.Combat.SelectSkill(dist);
      if (skill != null)
      {
        Vector2 dir = (target.position - enemy.transform.position).normalized;
        enemy.Combat.UseSkill(skill, dir);
      }

      yield return Wait(recoveryTime);
    }
  }
}