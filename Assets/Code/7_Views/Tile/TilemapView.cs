using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapView : MonoBehaviour, ITilemapView
{
    [SerializeField] private Tilemap targetTilemap;

    public void SetTile(Vector2Int position, TileBase tileToPlace)
    {
        targetTilemap.SetTile((Vector3Int)position, tileToPlace);
    }

    public void RemoveTile(Vector2Int position)
    {
        targetTilemap.SetTile((Vector3Int)position, null);
    }

    public void ClearAllTiles()
    {
        targetTilemap.ClearAllTiles();
    }
}