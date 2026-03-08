using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new SeedItem", menuName = "Item/SeedItem")]
public class SeedItem : Item
{
    public override EItemCategory Category => EItemCategory.Seed;
    public override EItemRole Role => EItemRole.Placeable;
    public override int MaxStackSize => 64;
}
