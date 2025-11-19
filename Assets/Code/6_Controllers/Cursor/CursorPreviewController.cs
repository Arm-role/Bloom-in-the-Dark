using System.Collections;
using UnityEngine;

public class CursorPreviewController : ICurserPreviewController
{
    private ICursorItemDisplay cursorItemDisplay;

    public void Setup(IItemInstance item)
    {
        cursorItemDisplay.Show(item.Data.Icon);
        cursorItemDisplay.SetColor(Color.white);
    }
    public void EnablePreview(Vector2 mousePosition)
    {
        throw new System.NotImplementedException();
    }
    public void UpdatePreview(IPointerResolver pointerResolver)
    {
        throw new System.NotImplementedException();
    }
    public void DisablePreview()
    {
        cursorItemDisplay.Hide();
    }

    public void UpdatePreview(Vector2 mousePosition)
    {
        throw new System.NotImplementedException();
    }
}