using System.Collections;
using UnityEngine;

public interface IItemInstance
{
    public IItemData ItemData { get; }
    public float Weight { get;   }
    public float CurrentLifetime { get; }
    public void ReduceLifetime(float amount);
}
