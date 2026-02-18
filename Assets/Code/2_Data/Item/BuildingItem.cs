using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new BuildingItem", menuName = "Item/New BuildingItem")]
public class BuildingItem : Item,  IItemData
{
    public override EItemCategory Category => EItemCategory.Building;
    public override EItemRole Role => EItemRole.Placeable;
    public override int MaxStackSize => 16;
}
