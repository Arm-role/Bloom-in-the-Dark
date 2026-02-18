using System.Collections;
using UnityEngine;

public interface IItemInstance
{
    IItemData Data { get; }
    int Level { get; }

    float GetMultiplier(StatKey key);
    float GetFlatBonus(StatKey key);
}