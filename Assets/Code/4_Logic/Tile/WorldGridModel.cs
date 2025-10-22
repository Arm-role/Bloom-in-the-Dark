using System;
using System.Collections.Generic;
using UnityEngine;

public class WorldGridModel : IWorldGridModel
{
    private Dictionary<Vector2Int, WorldTileData> _placedTiles = new Dictionary<Vector2Int, WorldTileData>();

    public Action<WorldTileData> OnTilePlaced { get; set; }
    public Action<Vector2Int> OnTileRemoved { get; set; }

    public void PlaceTile(Vector2Int position, string tileID)
    {
        var tileData = new WorldTileData { Position = position, TileID = tileID };
        _placedTiles[position] = tileData; 
        OnTilePlaced?.Invoke(tileData);
    }

    public void RemoveTile(Vector2Int position)
    {
        if (_placedTiles.ContainsKey(position))
        {
            _placedTiles.Remove(position);
            OnTileRemoved?.Invoke(position);
        }
    }

    public bool HasTileAt(Vector2Int position)
    {
        return _placedTiles.ContainsKey(position);
    }

    // --- ISaveable Implementation ---

    public object CaptureState()
    {
        // สร้าง List ของข้อมูลทั้งหมดเพื่อนำไปเซฟ
        var allTiles = new List<WorldTileData>(_placedTiles.Values);
        return allTiles;
    }

    public void RestoreState(object state)
    {
        if (state is List<WorldTileData> savedTiles)
        {
            _placedTiles.Clear();
            foreach (var tileData in savedTiles)
            {
                // ไม่ยิง Event ตอน Restore เพื่อประสิทธิภาพ
                // เราจะสั่งให้ View วาดใหม่ทั้งหมดทีเดียว
                _placedTiles[tileData.Position] = tileData;
            }

            // อาจจะยิง Event ใหญ่ครั้งเดียวว่า "โหลดเสร็จแล้วนะ"
            // OnWorldRestored?.Invoke();
        }
    }
}