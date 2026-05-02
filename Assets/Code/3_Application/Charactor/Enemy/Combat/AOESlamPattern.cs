// AOESlamPattern.cs — loop ของตัวเอง
using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "EnemyPattern/AOESlam")]
public class AOESlamPattern : EnemyPattern
{
  public float approachEdgeDist = 1.2f;
  public float windupTime = 0.6f;
  public float recoveryTime = 0.8f;

  public override IEnumerator Run(EnemyController enemy)
  {
    while (IsValid(enemy))
    {
      yield return ApproachUntil(enemy, approachEdgeDist);
      if (!IsValid(enemy)) yield break;

      yield return Windup(enemy, windupTime);
      if (!IsValid(enemy)) yield break;

      var skill = enemy.Combat.GetSkill<AOESlamSkill>();
      if (skill != null && skill.IsReady)
        enemy.Combat.UseSkill(skill, Vector2.zero);

      yield return Wait(recoveryTime);
    }
  }
}