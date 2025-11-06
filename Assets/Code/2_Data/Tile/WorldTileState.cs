using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WorldTileState
{
    private readonly Dictionary<Vector3Int, BaseTileData> _tileDataMap = new();

    public void SetTileData(Vector3Int cellPos, BaseTileData data) => _tileDataMap[cellPos] = data;
    public void ClearTileData(Vector3Int cellPos) => _tileDataMap.Remove(cellPos);
    public bool TryGetTileData(Vector3Int cellPos, out BaseTileData data) => _tileDataMap.TryGetValue(cellPos, out data);
    public int Count => _tileDataMap.Count;
}
