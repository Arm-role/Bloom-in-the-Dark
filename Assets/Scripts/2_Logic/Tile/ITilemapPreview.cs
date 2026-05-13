using UnityEngine;

public interface ITilemapPreview
{
    void ShowPreview(Vector3Int cellPos, bool canPlace);
    void HidePreview();
}
