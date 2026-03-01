using UnityEngine;

[CreateAssetMenu(menuName = "Skill/AreaCircleSkill")]
public class AreaCircleSkillDefinition : SkillDefinition
{
  [Header("Key")] 
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
  [SerializeField] private float baseRadius;
  [SerializeField] private float xAngle;


  public override bool Execute(IItemInstance instance, out ISkillDataPayload payload)
  {
    payload = new AreaCirclePayload
    {
      Damage =
        baseDamage * instance.Level +
        instance.GetFlatBonus(damageKey) +
        baseDamage * instance.GetMultiplier(damageKey),

      Range = baseRange,
      Radius = baseRadius,

      KnockForce =
        baseKnockForce * instance.Level +
        instance.GetFlatBonus(knockForceKey) +
        baseKnockForce * instance.GetMultiplier(knockForceKey),

      KnockDuration =
        baseKnockDuration * instance.Level +
        instance.GetFlatBonus(knockDurationKey) +
        baseKnockDuration * instance.GetMultiplier(knockDurationKey),

      Duration =
        baseDuration * instance.Level +
        instance.GetFlatBonus(durationKey) +
        baseDuration * instance.GetMultiplier(durationKey),
        
      Cooldown =
        baseCooldown * instance.Level +
        instance.GetFlatBonus(cooldownKey) +
        baseCooldown * instance.GetMultiplier(cooldownKey),
      
      XAngle = xAngle
    };
    
    return true; 
  }
}