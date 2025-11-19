using System;
using System.Threading.Tasks;
using UnityEngine;

public class CropPlacementSystem
{
    private readonly TilemapService _tileService;
    private readonly GameObjectSpawner _spawner;

    public CropPlacementSystem(TilemapService tileService, GameObjectSpawner spawner)
    {
        _tileService = tileService;
        _spawner = spawner;
    }

    public async Task<bool> TryPlantAtWorld(string plantName, Vector2 worldPos, TileBaseDataState tileState)
    {
        Vector3Int cell = _tileService.GetTilemap().WorldToCell(worldPos);
        Vector3 center = _tileService.GetTilemap().GetCellCenterWorld(cell);

        tileState.placedObject = await _spawner.SpawnOB(plantName, center);

        return true;
    }
}