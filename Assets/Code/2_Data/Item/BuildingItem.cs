using UnityEngine;

[CreateAssetMenu(fileName = "new BuildingItem", menuName = "Item/New BuildingItem")]
public class BuildingItem : Item, IBuildItemData
{
    [Header("BuildingData")]
    [SerializeField] private int buildingCout;
    [SerializeField] private Vector2Int gridSize;
    [SerializeField] private int energyReduceEachAction;

    public int BuildingCout => buildingCout;
    public Vector2Int GridSize => gridSize;
    public float EnergyReduceEachAction => energyReduceEachAction;

    public override EItemType Type => EItemType.Building;
    public override EItemStategyType StategyType => EItemStategyType.GridBased;
    public override int MaxStackSize => 12;

}