using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "EnemyPattern/BossAttack")]
public class BossAttackPattern : EnemyPattern
{
  public float recoveryTime = 0.6f;

  public override IEnumerator Run(EnemyController enemy)
  {
    while (IsValid(enemy))
    {
      float approachDist = enemy.Combat.GetMaxAttackRange() - 0.5f;
      yield return ApproachUntil(enemy, approachDist);

      if (!IsValid(enemy)) yield break;

      var target = Target(enemy);
      float dist = EdgeDist(enemy, target);

      var skill = enemy.Combat.SelectSkill(dist);
      if (skill != null)
      {
        Vector2 dir = (target.position - enemy.transform.position).normalized;
        enemy.Combat.UseSkill(skill, dir);

        // รอให้ skill เล่นจบก่อนค่อย loop ต่อ
        yield return new WaitUntil(() => !skill.IsExecuting);
      }

      yield return Wait(recoveryTime);
    }
  }
}
