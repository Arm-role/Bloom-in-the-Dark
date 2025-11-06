using UnityEngine;

public interface IInteractable
{
    float InteractionPriority { get; } 
    bool CanInteract(InteractionContext context);
    void OnHoverEnter();
    void OnHoverExit();
}
