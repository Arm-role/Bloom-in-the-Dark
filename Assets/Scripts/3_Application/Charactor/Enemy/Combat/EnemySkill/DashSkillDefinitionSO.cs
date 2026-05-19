using UnityEngine;

[CreateAssetMenu(menuName = "EnemySkill/Dash")]
public class DashSkillDefinitionSO : SkillDefinitionSO
{
  public float dashSpeed = 6f;
  public float duration = 1f;
  public float damage = 10f;
  public float cooldown = 2f;
  public float minRange = 2f;
  public float maxRange = 4f;
  public float prepareTime = 0.25f;

  public override IEnemySkill Create(LayerMask targetMask)
      => new DashSkill(dashSpeed, duration, damage, cooldown, minRange, maxRange, targetMask, prepareTime);
}
