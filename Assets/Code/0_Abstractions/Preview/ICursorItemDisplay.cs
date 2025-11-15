using UnityEngine;

public interface ICursorItemDisplay
{
    void SetColor(Color color);
    void Show(Sprite icon);
    void Hide();
}