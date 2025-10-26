using UnityEngine;

public interface ITargetDetector
{
    void Setup(InteractionHandleContext context);
    IDataProvider IntercationExcute(InteractionHandleContext context);
    void EnablePreview(InteractionHandleContext context);
    void UpdatePreview(InteractionHandleContext context);
    void DisablePreview();
}
