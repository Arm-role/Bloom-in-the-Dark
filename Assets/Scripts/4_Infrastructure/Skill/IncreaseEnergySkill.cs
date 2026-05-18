using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ItemModules/Skill/Increase Energy")]
public class IncreaseEnergySkill : SkillDefinition
{
  [Header("StatKey")]
  [SerializeField] private StatKey addEnergyKey;
  [SerializeField] private StatKey cooldownKey;

  [Header("SkillValue")]
  [SerializeField] private float baseIncrease;
  [SerializeField] private float baseCooldown;

  protected override void BuildStatMap()
  {
    _baseStats = new Dictionary<StatKey, float>
    {
        { addEnergyKey, baseIncrease },
        { cooldownKey, baseCooldown }
    };
  }

  public override bool Execute(IItemInstance instance, out ISkillDataPayload payload)
  {
    var stat = instance.Stats;

    payload = new IncreaseEnergyPayload
    {
      Increase = stat.GetStat(addEnergyKey),
      Cooldown = stat.GetStat(cooldownKey),
    };  

    return true;
  }
}