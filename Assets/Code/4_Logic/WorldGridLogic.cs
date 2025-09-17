using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldGridLogic
{
    private readonly HashSet<Vector2> _occupiedCells = new HashSet<Vector2>();

    public bool[,] GetPlacementValidity(Vector2Int originGridPos, Vector2Int size, Func<Vector2, Vector3> gridToWorld)
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

    public List<Vector2> PlaceObjectAt(List<PreviewTileInfo> tileInfos)
    {
        var tiles = new List<Vector2>();
        
        for (int i = 0; i < tileInfos.Count; i++)
        {
            var tilePos = tileInfos[i].WorldPosition;

            tiles.Add(new Vector2(tilePos.x, tilePos.y));
            Debug.Log($"Press { new Vector2(tilePos.x, tilePos.y)}");
            _occupiedCells.Add(new Vector2(tilePos.x, tilePos.y));
        }

        return tiles;   
    }
}