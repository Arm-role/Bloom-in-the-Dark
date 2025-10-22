using UnityEngine;

[CreateAssetMenu(fileName = "new ToolItem", menuName = "Item/New ToolItem")]
public class ToolItem : Item, IToolItemData
{
    [SerializeField] private EItemStategyType stategyType;

    [Header("PlantData")]
    [SerializeField] private int level;
    [SerializeField] private int energyReduce;

    public int Level => level;
    public float EnergyReduce => energyReduce;

    public override EItemType Type => EItemType.Tool;
    public override EItemStategyType StategyType => stategyType;
    public override int MaxStackSize => 1;

}
