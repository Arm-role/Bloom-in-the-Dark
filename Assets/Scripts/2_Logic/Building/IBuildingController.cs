using System;
using UnityEngine;

public interface IBuildingController
{
    bool IsAlive { get; }
    void Initialize(Action<GameObject> removeObject);
}
