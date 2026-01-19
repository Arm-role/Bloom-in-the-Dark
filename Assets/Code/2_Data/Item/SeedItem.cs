using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new SeedItem", menuName = "Item/New SeedItem")]
public class SeedItem : Item
{
    public override EItemType Type => EItemType.Seed;
    public override int MaxStackSize => 64;
}