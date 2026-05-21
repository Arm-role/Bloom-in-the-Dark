using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/PoisonAreaSkill")]
public class PoisonAreaSkillDefinition : SkillDefinition
{
  [Header("StatKey")]
  [SerializeField] private StatKey damageKey;
  [SerializeField] private StatKey radiusKey;
  [SerializeField] private StatKey durationKey;
  [SerializeField] private StatKey tickIntervalKey;
  [SerializeField] private StatKey cooldownKey;

  [Header("SkillValue")]
  [SerializeField] private float baseDamagePerTick;
  [SerializeField] private float baseRadius;
  [SerializeField] private float baseDuration;
  [SerializeField] private float baseTickInterval;
  [SerializeField] private float baseCooldown;

  [Header("SkillValueFix")]
  [SerializeField] private float baseRange;

  protected override void BuildStatMap()
  {
    _baseStats = new Dictionary<StatKey, float>
    {
        { damageKey, baseDamagePerTick },
        { radiusKey, baseRadius },
        { durationKey, baseDuration },
        { tickIntervalKey, baseTickInterval },
        { cooldownKey, baseCooldown }
    };
  }

  public override bool Execute(IItemInstance instance, out ISkillDataPayload payload)
  {
    var stat = instance.Stats;

    payload = new PoisonAreaPayload
    {
      DamagePerTick = stat.GetStat(damageKey),
      Radius = stat.GetStat(radiusKey),
      Duration = stat.GetStat(durationKey),
      TickInterval = stat.GetStat(tickIntervalKey),
      Cooldown = stat.GetStat(cooldownKey),

      Range = baseRange,
      XAngle = CameraConfig.XAngle
    };

    return true;
  }
}
