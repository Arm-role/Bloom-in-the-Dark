#nullable enable
using System.Collections.Generic;
using UnityEngine;

// Single source of truth for world cell storage.
// Pure storage — no zone/placement policy, no Unity lifecycle — so it stays
// simple and the cell map can be shared with read-only collaborators.
public sealed class WorldCellRegistry
{
  private readonly Dictionary<Vector3Int, WorldCell> _cells = new();

  public IReadOnlyDictionary<Vector3Int, WorldCell> Cells => _cells;

  public IEnumerable<WorldCell> All => _cells.Values;

  public WorldCell? Get(Vector3Int cellPos)
    => _cells.TryGetValue(cellPos, out var cell) ? cell : null;

  public void Add(Vector3Int cellPos, WorldCell cell)
    => _cells.Add(cellPos, cell);

  public bool Remove(Vector3Int cellPos)
    => _cells.Remove(cellPos);

  public void Clear() => _cells.Clear();
}
