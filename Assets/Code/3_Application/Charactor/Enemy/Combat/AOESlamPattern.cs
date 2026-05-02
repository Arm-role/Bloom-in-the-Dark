// AOESlamPattern.cs — loop ของตัวเอง
using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "EnemyPattern/AOESlam")]
public class AOESlamPattern : EnemyPattern
{
  public override IEnumerator Run(EnemyController enemy)
  {
    while (IsValid(enemy))
    {
      var target = Target(enemy);
      float dist = EdgeDist(enemy, target);

      var skill = enemy.Combat.SelectSkill(dist);

      if (skill == null)
      {
        yield return null;
        continue;
      }

      Vector2 dir = (target.position - enemy.transform.position).normalized;
      enemy.Combat.UseSkill(skill, dir);

      if (skill is AOESlamSkill)
      {
        yield return new WaitUntil(() => skill.IsReady || !IsValid(enemy));
      }
      else
      {
        yield return null;
      }
    }
  }
}