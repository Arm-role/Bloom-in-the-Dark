using UnityEngine;

public interface ICurserPreviewController
{
    public void Setup(IItemInstance item);
    void EnablePreview(Vector2 mousePosition);
    void UpdatePreview(Vector2 mousePosition);
    void DisablePreview();
}
