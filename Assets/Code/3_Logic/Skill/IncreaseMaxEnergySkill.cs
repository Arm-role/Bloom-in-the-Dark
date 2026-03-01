using UnityEngine;

[CreateAssetMenu(menuName = "ItemModules/Skill/Increase Max Energy")]
public class IncreaseMaxEnergySkill : SkillDefinition
{
  [Header("Key")] 
  [SerializeField] private StatKey maxEnergyKey;
  
  [Header("SkillValue")] 
  [SerializeField] private float baseIncrease;

  public override bool Execute(IItemInstance instance, out ISkillDataPayload payload)
  {
    payload = new IncreaseMaxEnergyPayload
    {
      Increase = baseIncrease * instance.Level +
                 instance.GetFlatBonus(maxEnergyKey) +
                 baseIncrease * instance.GetMultiplier(maxEnergyKey)
    };

    return true;
  }
}