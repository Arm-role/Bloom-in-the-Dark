// AOESlamSkillDefinitionSO.cs
using UnityEngine;

[CreateAssetMenu(menuName = "EnemySkill/AOESlam")]
public class AOESlamSkillDefinitionSO : SkillDefinitionSO
{
  [Header("Range")]
  public float minRange = 0f;
  public float maxRange = 5f;
  public float hitRadius = 1.8f;

  [Header("Stats")]
  public float damage = 15f;
  public float cooldown = 2.5f;

  [Header("Timing")]
  public float windupTime = 0.45f;
  public float riseDuration = 0.4f;   // ลอยขึ้นช้า
  public float fallDuration = 0.2f;   // พุ่งลงเร็ว
  public float recoveryTime = 0.5f;

  public override IEnemySkill Create(LayerMask targetMask)
      => new AOESlamSkill(
          minRange, maxRange, hitRadius,
          damage, cooldown,
          targetMask,
          windupTime, riseDuration, fallDuration, recoveryTime);
}