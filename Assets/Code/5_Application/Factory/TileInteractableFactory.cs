using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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
        List<ITileAction> actions = new();

        var flags = tileData.WorldInteractableType;

        if (flags.HasFlag(ETileBlockType.Tillable))
            actions.Add(new TillableAction(_tilemapService));

        if (flags.HasFlag(ETileBlockType.Plantable))
            actions.Add(new PlantableAction(_spawner));

        if (flags.HasFlag(ETileBlockType.Waterable))
            actions.Add(new WaterableAction());

        return new CompositeTileInteractable(state, actions);
    }

    public IWorldInteractable SetStrategy(ETileBlockType type, TileBaseDataState state)
    {
        if (type == ETileBlockType.None) return null;

        List<ITileAction> actions = new();

        if (type.HasFlag(ETileBlockType.Tillable))
            actions.Add(new TillableAction(_tilemapService));

        if (type.HasFlag(ETileBlockType.Plantable))
            actions.Add(new PlantableAction(_spawner));

        if (type.HasFlag(ETileBlockType.Waterable))
            actions.Add(new WaterableAction());

        return new CompositeTileInteractable(state, actions);
    }
}