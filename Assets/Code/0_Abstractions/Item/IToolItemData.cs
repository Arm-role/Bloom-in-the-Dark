using UnityEngine;

public interface IToolItemData : IEnergyReduce
{
    public int Level { get; }
    public float AttackRange { get; }
}
