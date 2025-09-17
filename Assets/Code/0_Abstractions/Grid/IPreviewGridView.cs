using UnityEngine;

public interface IPreviewGridView
{
    void AddTile(GameObject instance);
    void UpdateTile(int index, Vector3 position, Color color);
    void HideAllTiles();

}