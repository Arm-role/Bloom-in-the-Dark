using UnityEngine;

[CreateAssetMenu(fileName = "new ToolItem", menuName = "Item/New ToolItem")]
public class ToolItem : Item, IToolItemData
{
    [SerializeField] private EItemStategyType stategyType;

    [Header("ToolData")]
    [SerializeField] private int level;
    [SerializeField] private float attackRange;
    [SerializeField] private int energyReduceEachAction;

    public int Level => level;  
    public float AttackRange => attackRange;
    public float EnergyReduceEachAction => energyReduceEachAction;

    public override EItemType Type => EItemType.Tool;
    public override EItemStategyType StategyType => stategyType;
    public override int MaxStackSize => 1;

    public bool HasBonus { get; set; }
}
