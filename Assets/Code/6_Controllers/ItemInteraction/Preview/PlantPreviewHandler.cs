using UnityEngine;

public class PlantPreviewHandler : IInteractionHandle
{

    public PlantPreviewHandler()
    {
    }

    public void Setup(IItemInstance itemInstance)
    {
    }
    public bool IntercationExcute(IItemInstance itemInstance, Vector2 playerPosition, Vector2 pointerPosition, Collider2D target = null)
    {
        return false;
    }

    public void EnablePreview(Vector2 playerPosition, Vector2 pointerPosition)
    {
    }

    public void UpdatePreview(Vector2 playerPosition, Vector2 pointerPosition)
    {
    }

    public void DisablePreview()
    {
    }

    public bool CanInteraction(IItemInstance itemInstance)
    {
        throw new System.NotImplementedException();
    }
}