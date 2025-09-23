using System;

public interface IDragDropController
{
    event Action OnRequestDisable;
    event Action<InteractionContext> OnInteraction;
}