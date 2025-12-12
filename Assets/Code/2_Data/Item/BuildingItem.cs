using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new BuildingItem", menuName = "Item/New BuildingItem")]
public class BuildingItem : Item, IBuildItemData
{
    [SerializeField] private InputStrategyBinding[] strategyBindings;

    [Header("BuildingData")]
    [SerializeField] private int buildingCout;
    [SerializeField] private Vector2Int gridSize;
    [SerializeField] private int energyReduceEachAction;

    public int BuildingCout => buildingCout;
    public Vector2Int GridSize => gridSize;
    public float EnergyReduceEachAction => energyReduceEachAction;

    public override EItemType Type => EItemType.Building;
    public override IReadOnlyList<InputStrategyBinding> StrategyBindings => strategyBindings;
    public override int MaxStackSize => 16;
}
