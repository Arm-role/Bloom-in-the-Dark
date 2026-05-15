using System;

public interface IDragDropController
{
    event Action<InteractionContext> OnInteraction;
    InputActionType CurrentHeldActions { get; }
    HoverState CurrentHoverState { get; }
    void ManualUpdate();
}
