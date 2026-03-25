using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/AreaCircleSkill")]
public class AreaCircleSkillDefinition : SkillDefinition
{
  [Header("SkillKey")]
  [SerializeField] private string skillId;

  [Header("StatKey")]
  [SerializeField] private StatKey damageKey;
  [SerializeField] private StatKey knockForceKey;
  [SerializeField] private StatKey knockDurationKey;
  [SerializeField] private StatKey durationKey;
  [SerializeField] private StatKey cooldownKey;
  [SerializeField] private StatKey radiusKey;

  [Header("SkillValue")]
  [SerializeField] private float baseDamage;
  [SerializeField] private float baseKnockForce;
  [SerializeField] private float baseKnockDuration;
  [SerializeField] private float baseDuration;
  [SerializeField] private float baseCooldown;
  [SerializeField] private float baseRadius;

  [Header("SkillValueFix")]
  [SerializeField] private float baseRange;
  [SerializeField] private float xAngle;

  protected override void BuildStatMap()
  {
    _baseStats = new Dictionary<StatKey, float>
    {
        { damageKey, baseDamage },
        { knockForceKey, baseKnockForce },
        { knockDurationKey, baseKnockDuration },
        { durationKey, baseDuration },
        { cooldownKey, baseCooldown },
        { radiusKey, baseRadius }
    };
  }

  public override string SkillId => skillId;

  public override bool Execute(IItemInstance instance, out ISkillDataPayload payload)
  {
    var stat = instance.Stats;

    payload = new AreaCirclePayload
    {
      Damage = stat.GetStat(damageKey),
      Radius = stat.GetStat(radiusKey),
      KnockForce = stat.GetStat(knockForceKey),
      KnockDuration = stat.GetStat(knockDurationKey),
      Duration = stat.GetStat(durationKey),
      Cooldown = stat.GetStat(cooldownKey),

      Range = baseRange,
      XAngle = xAngle
    };

    Debug.Log(payload.Cooldown);

    return true;
  }
}