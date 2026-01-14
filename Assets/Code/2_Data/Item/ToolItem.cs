using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new ToolItem", menuName = "Item/New ToolItem")]
public class ToolItem : Item
{
    public override EItemType Type => EItemType.Tool;
    public override int MaxStackSize => 1;
    public bool HasBonus { get; set; }
}
