using System;
using UnityEngine;

public interface IDestructible
{
    event Action<GameObject> OnRequestDestruction;
    void RequestDestruction();
}