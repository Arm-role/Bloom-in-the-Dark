using UnityEngine;
using UnityEngine.Tilemaps;

public interface ITilemapView
{
    void SetTile(Vector2Int position, TileBase tileToPlace);
    void RemoveTile(Vector2Int position);
    void ClearAllTiles();
}