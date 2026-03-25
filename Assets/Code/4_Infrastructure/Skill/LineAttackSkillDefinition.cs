using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/LineAttackSkillDefinition")]
public class LineAttackSkillDefinition : SkillDefinition
{
  [Header("SkillKey")]
  [SerializeField] private string skillId;

  [Header("StatKey")]
  [SerializeField] private StatKey damageKey;
  [SerializeField] private StatKey knockForceKey;
  [SerializeField] private StatKey knockDurationKey;
  [SerializeField] private StatKey durationKey;
  [SerializeField] private StatKey cooldownKey;

  [Header("SkillValue")] 
  [SerializeField] private float baseDamage;
  [SerializeField] private float baseKnockForce;
  [SerializeField] private float baseKnockDuration;
  [SerializeField] private float baseDuration;
  [SerializeField] private float baseCooldown;

  [Header("SkillValueFix")] 
  [SerializeField] private float baseRange;
  [SerializeField] private float baseWidth;

  public override string SkillId => skillId;

  protected override void BuildStatMap()
  {
    _baseStats = new Dictionary<StatKey, float>
    {
        { damageKey, baseDamage },
        { knockForceKey, baseKnockForce },
        { knockDurationKey, baseKnockDuration },
        { durationKey, baseDuration },
        { cooldownKey, baseCooldown }
    };
  }

  public override bool Execute(IItemInstance instance, out ISkillDataPayload payload)
  {
    var stat = instance.Stats;

    payload = new LineAttackPayload
    {
      Damage = stat.GetStat(damageKey),
      KnockForce = stat.GetStat(knockForceKey),
      KnockDuration = stat.GetStat(knockDurationKey),
      Duration = stat.GetStat(durationKey),
      Cooldown = stat.GetStat(cooldownKey),

      Range = baseRange,
      Width = baseWidth
    };

    return true;
  }
}