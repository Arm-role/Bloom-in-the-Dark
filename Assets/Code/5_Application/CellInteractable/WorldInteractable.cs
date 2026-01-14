using System;
using UnityEngine;

public class WorldInteractable : 
    MonoBehaviour,
    IDestructible,
    IPoolable<GameObject>
{
    public event Action<GameObject> OnRequestDestruction;
    public void RequestDestruction()
    {
        OnRequestDestruction?.Invoke(gameObject);
    }

    public bool IsAlive { get; set; }
    public void OnSpawnFromPool(GameObject ob)
    {
        
    }

    public void OnReturnToPool(GameObject ob)
    {
        
    }
}