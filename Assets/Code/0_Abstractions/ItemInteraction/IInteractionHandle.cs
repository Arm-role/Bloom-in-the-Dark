using UnityEngine;

public interface IInteractionHandle
{
    void Setup(InteractionHandleContext context);
    bool IntercationExcute(InteractionHandleContext context);
    void EnablePreview(InteractionHandleContext context);
    void UpdatePreview(InteractionHandleContext context);
    void DisablePreview();
}
