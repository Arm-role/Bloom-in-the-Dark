using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new PlantSkillCasterItem", menuName = "Item/PlantSkillCasterItem")]
public class PlantSkillCasterItem : Item
{
  [Header("Type")] [SerializeField] private EPlantType plantType;

  public override EItemCategory Category => EItemCategory.Plant;
  public override EItemRole Role => EItemRole.SkillCaster;
  public EPlantType PlantType => plantType;
  public override int MaxStackSize => 64;
}