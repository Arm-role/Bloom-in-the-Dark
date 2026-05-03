using UnityEngine;

[CreateAssetMenu(menuName = "EnemySkill/Melee")]
public class MeleeSkillDefinitionSO : SkillDefinitionSO
{
  public float range = 1.2f;
  public float damage = 3f;
  public float cooldown = 1f;
  public float windup = 0.25f;
  public float recovery = 0.45f;

  public override IEnemySkill Create(LayerMask targetMask)
      => new MeleeSkill(range, damage, cooldown, targetMask, windup, recovery);
}

[CreateAssetMenu(menuName = "Enemy/SkillSet")]
public class EnemySkillSetSO : ScriptableObject
{
  public SkillDefinitionSO[] skills;

  public void ApplyTo(EnemyController enemy, LayerMask targetMask)
  {
    foreach (var def in skills)
    {
      if (def == null) continue;
      enemy.AddSkill(def.Create(targetMask));
    }
  }
}