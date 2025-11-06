using System.Collections;
using UnityEngine;

public class PlantItemInstance : IItemInstance
{
    public IItemData ItemData { get; private set; }
    public float Weight { get; private set; }
    public float CurrentLifetime { get; private set; }

    public PlantItemInstance(IItemData itemData, float weight, float maxLifetime)
    {
        ItemData = itemData;
        Weight = weight;

        CurrentLifetime = maxLifetime;
    }

    public void ReduceLifetime(float amount)
    {
        if (amount <= 0) return;

        CurrentLifetime -= amount;

        if (CurrentLifetime <= 0)
        {
            CurrentLifetime = 0;
        }
    }
}
