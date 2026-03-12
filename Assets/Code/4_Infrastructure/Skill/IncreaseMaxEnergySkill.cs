using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ItemModules/Skill/Increase Max Energy")]
public class IncreaseMaxEnergySkill : SkillDefinition
{
  [Header("SkillKey")]
  [SerializeField] private string skillId;

  [Header("StatKey")]
  [SerializeField] private StatKey maxEnergyKey;
  [SerializeField] private StatKey cooldownKey;

  [Header("SkillValue")]
  [SerializeField] private float baseIncrease;
  [SerializeField] private float baseCooldown;

  public override string SkillId => skillId;
  protected override void BuildStatMap()
  {
    _baseStats = new Dictionary<StatKey, float>
    {
        { maxEnergyKey, baseIncrease },
        { cooldownKey, baseCooldown }
    };
  }

  public override bool Execute(IItemInstance instance, out ISkillDataPayload payload)
  {
    payload = new IncreaseMaxEnergyPayload
    {
      Increase = instance.GetStat(maxEnergyKey),
      Cooldown = instance.GetStat(cooldownKey),
    };  

    return true;
  }
}