#nullable enable
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

// Scans a tilemap layer once at init, creating cells from existing tiles.
public sealed class TileLayerScanner
{
  private readonly ITileLibrary _tileLibrary;
  private readonly Func<Vector3Int, WorldCell> _getOrCreateCell;

  public TileLayerScanner(
    ITileLibrary tileLibrary,
    Func<Vector3Int, WorldCell> getOrCreateCell)
  {
    _tileLibrary = tileLibrary;
    _getOrCreateCell = getOrCreateCell;
  }

  public void Scan(ETileLayerType layerType, Tilemap tilemap)
  {
    var bounds = tilemap.cellBounds;

    for (int x = bounds.xMin; x < bounds.xMax; x++)
    {
      for (int y = bounds.yMin; y < bounds.yMax; y++)
      {
        var cellPos = new Vector3Int(x, y, 0);
        var tile = tilemap.GetTile(cellPos);
        if (tile == null)
          continue;

        var cell = _getOrCreateCell(cellPos);
        cell.AddTile(layerType, _tileLibrary.GetTileData(tile));
      }
    }

    TileDomainEvents.TileScanCompleted();
  }
}
