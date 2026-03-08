using UnityEngine;

[CreateAssetMenu(fileName = "new PlantConsumableItem", menuName = "Item/PlantConsumableItem")]
public class PlantConsumableItem : Item
{
  [Header("Type")] [SerializeField] private EPlantType plantType;

  public override EItemCategory Category => EItemCategory.Plant;
  public override EItemRole Role => EItemRole.Consumable;
  public EPlantType PlantType => plantType;
  public override int MaxStackSize => 64;
}