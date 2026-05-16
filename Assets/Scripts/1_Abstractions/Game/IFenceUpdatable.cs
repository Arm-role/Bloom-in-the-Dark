using System;
using UnityEngine;

public interface IFenceUpdatable
{
    void Initialize(Vector3Int cellPos, Func<Vector3Int, bool> isFenceAt);
    void UpdateBitmask();
}
