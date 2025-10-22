using UnityEngine;

public interface IBuildItemData
{
    public int BuildingCout { get; }
    public Vector2Int GridSize { get; }
    public float EnergyReduceEachAction { get; }
}