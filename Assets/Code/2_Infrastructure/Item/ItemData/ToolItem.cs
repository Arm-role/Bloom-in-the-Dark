using UnityEngine;

[CreateAssetMenu(fileName = "new ToolItem", menuName = "Item/ToolItem")]
public class ToolItem : Item
{
  [Header("Type")] [SerializeField] private EToolType toolType;

  public override EItemCategory Category => EItemCategory.Tool;
  public override EItemRole Role => EItemRole.Tool;
  public EToolType ToolType => toolType;

  public override int MaxStackSize => 1;
  public bool HasBonus { get; set; }
}