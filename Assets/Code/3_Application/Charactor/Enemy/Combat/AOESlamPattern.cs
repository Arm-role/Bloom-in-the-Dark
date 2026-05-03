// AOESlamPattern.cs
using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "EnemyPattern/AOESlam")]
public class AOESlamPattern : EnemyPattern
{
  public float delayBetweenSkills = 1f;  

  public override IEnumerator Run(EnemyController enemy)
  {
    while (IsValid(enemy))
    {
      var target = Target(enemy);
      float dist = EdgeDist(enemy, target);

      var skill = enemy.Combat.SelectSkill(dist);
      if (skill == null) { yield return null; continue; }

      Vector2 dir = (target.position - enemy.transform.position).normalized;
      enemy.Combat.UseSkill(skill, dir);

      // ✅ รอแค่ให้ execute จบ ไม่รอ cooldown
      yield return new WaitUntil(() => !skill.IsExecuting || !IsValid(enemy));

      yield return Wait(delayBetweenSkills);
    }
  }
}