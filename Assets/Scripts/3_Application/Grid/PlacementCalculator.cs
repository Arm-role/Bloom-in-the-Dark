using System.Collections.Generic;
using UnityEngine;

public class PlacementCalculator
{
    private readonly IGridConverter _gridConverter;
    private readonly IWorldGridLogic _worldGridLogic;
    private readonly float _maxDistance;

    public PlacementCalculator(IGridConverter gridLogic, IWorldGridLogic worldGridLogic, float maxDistance)
    {
        _gridConverter = gridLogic;
        _worldGridLogic = worldGridLogic;
        _maxDistance = maxDistance;
    }

    public List<TileInfo> Calculate(Vector2 playerPos, Vector2 pointerPos, Vector2Int size)
    {
        Vector2Int mouseGrid = _gridConverter.WorldToGrid(pointerPos);
        Vector2Int origin = mouseGrid - new Vector2Int(size.x / 2, size.y / 2);

        bool[,] validity = _worldGridLogic.GetPlacementValidity(origin, size, _gridConverter.GridToWorld);
        var list = new List<TileInfo>();

        for (int x = 0; x < size.x; x++)
            for (int y = 0; y < size.y; y++)
            {
                Vector2Int tileGrid = new(origin.x + x, origin.y + y);
                Vector3 worldPos = _gridConverter.GridToWorld(tileGrid);
                float distance = Vector2.Distance(playerPos, worldPos);

                PlacementState state =
                    distance > _maxDistance ? PlacementState.OutOfRange :
                    !validity[x, y] ? PlacementState.Blocked :
                    PlacementState.Valid;

                list.Add(new TileInfo
                {
                    WorldPosition = worldPos,
                    State = state
                });
            }

        return list;
    }
}