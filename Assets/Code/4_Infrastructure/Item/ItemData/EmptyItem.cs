using UnityEngine;

[CreateAssetMenu(fileName = "EmptyItem", menuName = "Item/EmptyItem")]
public class EmptyItem : Item
{
  public override EItemCategory Category => EItemCategory.None;
  public override EItemRole Role => EItemRole.None;
  public override int MaxStackSize => 1;
}