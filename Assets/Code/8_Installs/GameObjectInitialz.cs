using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameObjectInitialz
{
    public GameObjectInitialz(SpawnerHandle spawnerHandle)
    {
        spawnerHandle.OnSpawnCompleted += Initialze;
    }

    public async void Initialze(Task<GameObject> ob)
    {
        GameObject obj = await ob;
        
        if (obj.TryGetComponent<IItemInstance>(out var interactable))
        {

        }
    }
}
