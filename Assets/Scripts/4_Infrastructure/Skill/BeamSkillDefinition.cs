using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/BeamSkill")]
public class BeamSkillDefinition : SkillDefinition
{
  [Header("StatKey")]
  [SerializeField] private StatKey damageKey;
  [SerializeField] private StatKey knockForceKey;
  [SerializeField] private StatKey knockDurationKey;
  [SerializeField] private StatKey durationKey;
  [SerializeField] private StatKey tickIntervalKey;
  [SerializeField] private StatKey cooldownKey;

  [Header("SkillValue")]
  [SerializeField] private float baseDamagePerTick;
  [SerializeField] private float baseKnockForce;
  [SerializeField] private float baseKnockDuration;
  [SerializeField] private float baseDuration;
  [SerializeField] private float baseTickInterval;
  [SerializeField] private float baseCooldown;

  [Header("SkillValueFix")]
  [SerializeField] private float baseRange;
  [SerializeField] private float baseWidth;

  protected override void BuildStatMap()
  {
    _baseStats = new Dictionary<StatKey, float>
    {
        { damageKey, baseDamagePerTick },
        { knockForceKey, baseKnockForce },
        { knockDurationKey, baseKnockDuration },
        { durationKey, baseDuration },
        { tickIntervalKey, baseTickInterval },
        { cooldownKey, baseCooldown }
    };
  }

  public override bool Execute(IItemInstance instance, out ISkillDataPayload payload)
  {
    var stat = instance.Stats;

    payload = new BeamPayload
    {
      DamagePerTick = stat.GetStat(damageKey),
      KnockForce = stat.GetStat(knockForceKey),
      KnockDuration = stat.GetStat(knockDurationKey),
      Duration = stat.GetStat(durationKey),
      TickInterval = stat.GetStat(tickIntervalKey),
      Cooldown = stat.GetStat(cooldownKey),

      Range = baseRange,
      Width = baseWidth
    };

    return true;
  }
}
