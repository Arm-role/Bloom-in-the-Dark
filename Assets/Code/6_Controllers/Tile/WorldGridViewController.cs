using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldGridViewController : MonoBehaviour
{
    // --- Dependencies (Inject by Installer) ---
    private WorldGridModel _gridModel;
    private TileLibraryService _tileLibrary;
    private ITilemapView _tilemapView;

    public void Initialize(WorldGridModel gridModel, TileLibraryService tileLibrary, ITilemapView tilemapView)
    {
        _gridModel = gridModel;
        _tileLibrary = tileLibrary;
        _tilemapView = tilemapView;

        _gridModel.OnTilePlaced += HandleTilePlaced;
        _gridModel.OnTileRemoved += HandleTileRemoved;

        // (Optional) อาจจะมี Event สำหรับการวาดใหม่ทั้งหมดตอนโหลดเกม
        // _gridModel.OnWorldRestored += RedrawAllTiles;
    }

    private void OnDestroy()
    {
        if (_gridModel != null)
        {
            _gridModel.OnTilePlaced -= HandleTilePlaced;
            _gridModel.OnTileRemoved -= HandleTileRemoved;
        }
    }

    // เมื่อ Model เปลี่ยนแปลง
    private void HandleTilePlaced(WorldTileData tileData)
    {
        // 1. Controller "แปล" ID เป็น TileBase โดยใช้ Service
        TileBase tileToPlace = _tileLibrary.GetTileByID(tileData.TileID);

        // 2. Controller "สั่ง" View ให้วาด
        _tilemapView.SetTile(tileData.Position, tileToPlace);
    }

    private void HandleTileRemoved(Vector2Int position)
    {
        // สั่ง View ให้ลบ
        _tilemapView.RemoveTile(position);
    }

    // เมธอดสำหรับวาดใหม่ทั้งหมด
    public void RedrawAllTiles(Dictionary<Vector2Int, WorldTileData> allTiles)
    {
        _tilemapView.ClearAllTiles();
        foreach (var tileData in allTiles.Values)
        {
            HandleTilePlaced(tileData);
        }
    }
}