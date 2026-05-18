using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/ConeSkill")]
public class ConeSkillDefinition : SkillDefinition
{
  [Header("Stat Keys")]
  [SerializeField] private StatKey damageKey;
  [SerializeField] private StatKey knockForceKey;
  [SerializeField] private StatKey knockDurationKey;
  [SerializeField] private StatKey durationKey;
  [SerializeField] private StatKey cooldownKey;

  [Header("Base Values")]
  [SerializeField] private float baseDamage;
  [SerializeField] private float baseKnockForce;
  [SerializeField] private float baseKnockDuration;
  [SerializeField] private float baseDuration;
  [SerializeField] private float baseCooldown;

  [Header("SkillValueFix")]
  [SerializeField] private float baseRange;
  [SerializeField] private float baseAngle;

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

    var conePayload = new ConeAttackPayload
    {
      Damage = stat.GetStat(damageKey),
      KnockForce = stat.GetStat(knockForceKey),
      KnockDuration = stat.GetStat(knockDurationKey),
      Duration = stat.GetStat(durationKey),
      Cooldown = stat.GetStat(cooldownKey),

      Range = baseRange,
      AngleDeg = baseAngle,
      XAngle = CameraConfig.XAngle
    };

    if (!conePayload.IsValid)
    {
      Debug.LogError($"[ConeSkill] Invalid payload generated for {name}");
      payload = default;
      return false;
    }

    payload = conePayload;
    return true;
  }
}
