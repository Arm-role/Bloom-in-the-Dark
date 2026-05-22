#nullable enable
using System;
using System.Linq;
using UnityEngine;

// Runtime add/remove of tiles — keeps the cell model and the visual tilemap
// in sync.
public sealed class TileModificationService
{
  private readonly WorldCellRegistry _registry;
  private readonly TileLayerRenderer _renderer;
  private readonly Func<Vector3Int, WorldCell> _getOrCreateCell;

  public TileModificationService(
    WorldCellRegistry registry,
    TileLayerRenderer renderer,
    Func<Vector3Int, WorldCell> getOrCreateCell)
  {
    _registry = registry;
    _renderer = renderer;
    _getOrCreateCell = getOrCreateCell;
  }

  public bool TryAddTile(
    Vector3Int cellPos,
    ETileLayerType layer,
    IBaseTileData tileData)
  {
    var cell = _getOrCreateCell(cellPos);

    if (!cell.AddTile(layer, tileData))
      return false;

    _renderer.SetTile(cellPos, layer, tileData.Tiles.FirstOrDefault());
    return true;
  }

  public bool TryRemoveTile(Vector3Int cellPos, ETileLayerType layer)
  {
    var cell = _registry.Get(cellPos);
    if (cell == null)
      return false;

    if (!cell.RemoveTile(layer))
      return false;

    if (cell.IsEmpty)
      _registry.Remove(cellPos);

    _renderer.ClearTile(cellPos, layer);
    return true;
  }
}
