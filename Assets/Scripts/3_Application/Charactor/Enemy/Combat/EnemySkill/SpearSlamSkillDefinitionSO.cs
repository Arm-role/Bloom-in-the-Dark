using UnityEngine;

[CreateAssetMenu(menuName = "EnemySkill/SpearSlam")]
public class SpearSlamSkillDefinitionSO : SkillDefinitionSO
{
  public float minRange = 2f;
  public float maxRange = 5f;
  public float hitRange = 2.5f;
  public float hitWidth = 1.5f;
  public float arcHeight = 2.5f;
  public float damage = 25f;
  public float knockbackForce = 8f;
  public float cooldown = 3f;
  public float windupTime = 0.4f;
  public float riseDuration = 0.35f;
  public float fallDuration = 0.2f;
  public float recoveryTime = 0.5f;

  public override IEnemySkill Create(LayerMask targetMask) =>
      new SpearSlamSkill(minRange, maxRange, hitRange, hitWidth,
          arcHeight, damage, knockbackForce, cooldown, targetMask,
          windupTime, riseDuration, fallDuration, recoveryTime);
}
