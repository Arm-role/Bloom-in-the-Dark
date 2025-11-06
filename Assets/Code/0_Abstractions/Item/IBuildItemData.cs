using UnityEngine;

public interface IBuildItemData : IEnergyReduce
{
    public int BuildingCout { get; }
    public Vector2Int GridSize { get; }
}
