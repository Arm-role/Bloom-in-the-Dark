using UnityEngine;

[CreateAssetMenu(menuName = "EnemySkill/SpearDive")]
public class SpearDiveSkillDefinitionSO : SkillDefinitionSO
{
  public float minRange = 1f;
  public float maxRange = 6f;
  public float riseHeight = 3f;
  public float hitRadius = 0.6f;
  public float damage = 35f;
  public float knockbackForce = 5f;
  public float cooldown = 5f;
  public float windupTime = 0.3f;
  public float riseDuration = 0.5f;
  public float peakDuration = 0.4f;
  public float fallDuration = 0.15f;
  public float recoveryTime = 0.6f;

  public override IEnemySkill Create(LayerMask targetMask) =>
      new SpearDiveSkill(minRange, maxRange, riseHeight, hitRadius,
          damage, knockbackForce, cooldown, targetMask,
          windupTime, riseDuration, peakDuration, fallDuration, recoveryTime);
}
