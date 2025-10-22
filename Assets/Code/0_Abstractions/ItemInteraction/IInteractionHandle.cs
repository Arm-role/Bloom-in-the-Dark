using UnityEngine;

public interface IInteractionHandle
{
    void Setup(IItemInstance itemInstance);
    bool IntercationExcute(IItemInstance itemInstance, Vector2 playerPosition, Vector2 pointerPosition, Collider2D target = null);
    void EnablePreview(Vector2 playerPosition, Vector2 pointerPosition);
    void UpdatePreview(Vector2 playerPosition, Vector2 pointerPosition);
    void DisablePreview();
}