using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new PlantItem", menuName = "Item/New PlantItem")]
public class PlantItem : Item
{
    public override EItemType Type => EItemType.Plant;
    public override int MaxStackSize => 64;
}
