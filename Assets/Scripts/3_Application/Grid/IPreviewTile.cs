using UnityEngine;

public interface IPreviewTile
{
    void Show(Sprite sprite);
    void SetPosition(Vector3 worldPosition);
    void SetColor(Color color);
}
