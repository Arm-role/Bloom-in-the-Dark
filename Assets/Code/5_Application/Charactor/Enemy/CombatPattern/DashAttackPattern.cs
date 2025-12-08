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
        var locomotion = enemy.Locomotion;

        var dash = combat.GetSkill<DashSkill>();
        if (dash == null)
        {
            Debug.LogWarning("DashAttackPattern requires a DashSkill!");
            yield break;
        }

        float approachDistance = dash.MaxRange - 0.5f;

        while (true)
        {
            if (target == null) yield break;

            while (Vector2.Distance(enemy.transform.position, target.position) > approachDistance)
            {
                yield return null;
            }

            enemy.Locomotion.Stop();
            enemy.Combat.OnRequestStopMovement?.Invoke(windupTime);
            yield return new WaitForSeconds(windupTime);

            if (enemy.Data.IsDead) yield break;
            if (!target.GetComponent<IPoolable<GameObject>>().IsAlive) yield break;

            Vector2 dir = (target.position - enemy.transform.position).normalized;

            var skill = combat.SelectSkill(
                Vector2.Distance(enemy.transform.position, target.position)
            );
            if (skill != null)
                combat.UseSkill(skill, dir);

            yield return new WaitForSeconds(afterAttackWait);
        }
    }
}
