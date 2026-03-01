using System.Collections.Generic;
using UnityEngine;

public class PlacementController
{
    private readonly GridConverter _gridConverter;
    private readonly WorldGridLogic _worldGridLogic;

    private PlacementCalculator _calculator;
    private Vector2Int _size;

    public PlacementController(GridConverter gridConverter, WorldGridLogic worldGridLogic)
    {
        _gridConverter = gridConverter;
        _worldGridLogic = worldGridLogic;
    }

    public void Setup(Vector2Int size, float maxDistance)
    {
        _size = size;
        _calculator = new PlacementCalculator(_gridConverter, _worldGridLogic, maxDistance);
    }

    public List<TileInfo> GetTargetTilePreview(Vector3 playerPos, Vector3 pointerPos)
    {
        return _calculator.Calculate(playerPos, pointerPos, _size);
    }

    public List<TileInfo> HandlePlacementClick(IItemInstance item, Vector2 playerPos, Vector2 pointerPos)
    {
        var infos = _calculator.Calculate(playerPos, pointerPos, _size);

        bool canPlace = infos.TrueForAll(i => i.State == PlacementState.Valid);

        if (canPlace)
        {
            _worldGridLogic.PlaceObjectAt(infos);
            return infos;
        }

        return null;
    }
}
