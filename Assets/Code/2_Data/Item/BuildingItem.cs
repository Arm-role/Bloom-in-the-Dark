using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new BuildingItem", menuName = "Item/New BuildingItem")]
public class BuildingItem : Item,  IItemData
{
    public override EItemType Type => EItemType.Building;
    public override int MaxStackSize => 16;
}
