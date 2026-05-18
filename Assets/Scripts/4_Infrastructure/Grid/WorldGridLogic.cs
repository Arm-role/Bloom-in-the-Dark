using System;
using System.Collections.Generic;
using UnityEngine;

public class WorldGridLogic : IWorldGridLogic
{
    private readonly HashSet<Vector2> _occupiedCells = new HashSet<Vector2>();

    public bool[,] GetPlacementValidity(Vector2Int originGridPos, Vector2Int size, Func<Vector2Int, Vector3> gridToWorld)
    {
        bool[,] validityMap = new bool[size.x, size.y];

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector2Int tileGridPos = new Vector2Int(originGridPos.x + x, originGridPos.y + y);
                var worldPos = gridToWorld(tileGridPos);
                validityMap[x, y] = !_occupiedCells.Contains(worldPos);
            }
        }
        return validityMap;
    }
    
    public List<Vector2> PlaceObjectAt(List<TileInfo> tileInfos)
    {
        var tiles = new List<Vector2>();
        
        for (int i = 0; i < tileInfos.Count; i++)
        {
            var tilePos = tileInfos[i].WorldPosition;
    
            tiles.Add(new Vector2(tilePos.x, tilePos.y));
            _occupiedCells.Add(new Vector2(tilePos.x, tilePos.y));
        }
    
        return tiles;   
    }
    public List<Vector2> RemoveTiles(List<TileInfo> tileInfos)
    {
        var tiles = new List<Vector2>();
    
        for (int i = 0; i < tileInfos.Count; i++)
        {
            var tilePos = tileInfos[i].WorldPosition;
            Vector2 key = new Vector2(tilePos.x, tilePos.y);
    
            if (_occupiedCells.Contains(key))
            {
                _occupiedCells.Remove(key);
                tiles.Add(new Vector2(tilePos.x, tilePos.y));
            }
        }
        return tiles;
    }
}
