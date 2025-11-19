using UnityEngine;

public class TileInteractableFactory
{
    private GameSceneSettings _gameSceneSettings;

    private readonly TilemapService _tilemapService;
    private readonly SpawnerHandle _spawner;
    private readonly ParticalService _particalService;

    public TileInteractableFactory(
        GameSceneSettings gameSceneSettings,
        TilemapService tilemapService,
        SpawnerHandle spawner,
        ParticalService particalService)
    {
        _gameSceneSettings = gameSceneSettings;
        _tilemapService = tilemapService;
        _spawner = spawner;
        _particalService = particalService;
    }

    public IWorldInteractable Create(TileBaseData tileData, TileBaseDataState state)
    {
        if (tileData == null) return null;

        var type = tileData.WorldInteractableType;

        if (type.HasFlag(EWorldInteractableType.BuildableArea))
            return new SoilTileInteractable(state,_tilemapService, _spawner);

        if (type.HasFlag(EWorldInteractableType.Tillable))
            return new GrassTileInteractable(state, _tilemapService);

        return null;
    }

    public IWorldInteractable SetStrategy(EWorldInteractableType type, TileBaseDataState state)
    {
        if (type == EWorldInteractableType.None) return null;

        Debug.Log(type.ToString());

        if (type.HasFlag(EWorldInteractableType.BuildableArea))
            return new SoilTileInteractable(state, _tilemapService, _spawner);

        if (type.HasFlag(EWorldInteractableType.Tillable))
            return new GrassTileInteractable(state, _tilemapService);

        return null;
    }
}