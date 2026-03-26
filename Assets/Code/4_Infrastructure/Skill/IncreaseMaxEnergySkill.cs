using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ItemModules/Skill/Increase Max Energy")]
public class IncreaseMaxEnergySkill : SkillDefinition
{
  [Header("StatKey")]
  [SerializeField] private StatKey maxEnergyKey;
  [SerializeField] private StatKey cooldownKey;

  [Header("SkillValue")]
  [SerializeField] private float baseIncrease;
  [SerializeField] private float baseCooldown;

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
    var stat = instance.Stats;

    payload = new IncreaseMaxEnergyPayload
    {
      Increase = stat.GetStat(maxEnergyKey),
      Cooldown = stat.GetStat(cooldownKey),
    };  

    return true;
  }
}