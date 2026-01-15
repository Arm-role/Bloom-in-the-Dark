using System;

public interface IDragDropController
{
    event Action OnRequestDisable;
    event Action<InteractionContext> OnInteraction;
    InputActionType CurrentHeldActions { get; }
    void ManualUpdate();
}
