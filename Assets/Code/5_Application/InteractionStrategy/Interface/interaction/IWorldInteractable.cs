using System;
using System.Threading.Tasks;
using UnityEngine;

public interface IWorldInteractable
{
    float InteractionPriority { get; }
    EWorldInteractableType Type { get; }

    event Action<GameObject> OnRequestDestruction;
    Task<bool> TryInteract(InteractionHandleContext context);
    bool CanInteract(InteractionHandleContext context);
}
