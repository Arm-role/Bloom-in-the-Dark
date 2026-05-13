using System;

public interface IDragDropController
{
    event Action<InteractionContext> OnInteraction;
    InputActionType CurrentHeldActions { get; }
    void ManualUpdate();
}
