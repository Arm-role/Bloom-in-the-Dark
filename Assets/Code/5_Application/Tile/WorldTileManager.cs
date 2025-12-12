using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class WorldTileManager : MonoBehaviour
{
    private List<TilemapLayer> _tilemapLayers = new();

    private TileLibrary _tileLibrary;
    public GridConverter GridConverter;
    private TileInteractableFactory _interactableFactory;

    private Dictionary<Vector3Int, TileBaseDataState> _worldTiles = new();

    public void Initialize(
        List<TilemapLayer> tilemapLayers,
        TileLibrary tileLibrary,
        GridConverter gridConverter,
        TileInteractableFactory tileInteractableFactory)
    {
        _tilemapLayers = tilemapLayers;
        _tileLibrary = tileLibrary;
        _interactableFactory = tileInteractableFactory;

        GridConverter = gridConverter;

        _worldTiles.Clear();

        foreach (var layer in tilemapLayers)
        {
            if (layer.tilemap == null) continue;
            ScanLayer(layer.layerType, layer.tilemap);
        }

        ScanObstacles();
        Debug.Log($"✅ WorldTileManager initialized with {_worldTiles.Count} tiles");
    }

    private void ScanLayer(ETileLayerType layerType, Tilemap map)
    {
        var bounds = map.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                var cellPos = new Vector3Int(x, y, 0);
                var tile = map.GetTile(cellPos);
                if (tile == null) continue;

                // compute world center from tilemap
                Vector3 worldCenter = map.GetCellCenterWorld(cellPos);

                if (!_worldTiles.TryGetValue(cellPos, out var state))
                {
                    state = new TileBaseDataState(cellPos, worldCenter);
                    _worldTiles.Add(cellPos, state);
                }

                var tileData = _tileLibrary.GetTileData(tile);
                state.SetTile(layerType, tileData);

                state.WorldInteractable = _interactableFactory.Create(tileData, state);
                state.WorldInteractableType = tileData.WorldInteractableType;
            }
        }

        Debug.Log("ScanComplete");
        TileDomainEvents.TileScanCompleted();
    }
    public void ScanObstacles()
    {
        foreach (var state in _worldTiles.Values)
            state.ObstacleObject = null;

        TileObstacle[] obstacles = FindObjectsOfType<TileObstacle>();

        foreach (var ob in obstacles)
        {
            float cellSize = GridConverter.CellSize;

            // bottom-left จุดเริ่ม scan
            Vector3 obstacleBL = ob.GetObstacleBottomLeft(cellSize);
            Vector3Int baseCell = GridConverter.WorldToCell(obstacleBL);

            for (int x = 0; x < ob.ObstacleSize.x; x++)
            {
                for (int y = 0; y < ob.ObstacleSize.y; y++)
                {
                    Vector3Int cell = new(baseCell.x + x, baseCell.y + y, 0);

                    if (_worldTiles.TryGetValue(cell, out var state))
                    {
                        state.ObstacleObject = ob;
                    }
                }
            }
        }

        Debug.Log("📦 Obstacle scan complete. Count = " + obstacles.Length);

        TileDomainEvents.ObstacleScanCompleted();
    }

    public IEnumerable<TileBaseDataState> GetTileBaseDataStates() => _worldTiles.Values;

    public TileBaseDataState GetTileState(Vector3Int cellPos)
    {
        _worldTiles.TryGetValue(cellPos, out var state);
        return state;
    }

    public TileBaseDataState GetOrCreateTileState(Vector3Int pos, Tilemap tilemap = null)
    {
        if (!_worldTiles.TryGetValue(pos, out var state))
        {
            Vector3 worldCenter = tilemap != null
                ? tilemap.GetCellCenterWorld(pos)
                : Vector3.zero;

            state = new TileBaseDataState(pos, worldCenter);
            _worldTiles.Add(pos, state);
        }
        return state;
    }

    public bool TryGetTilemap(ETileLayerType layerType, out Tilemap tilemap)
    {
        foreach (var t in _tilemapLayers)
        {
            if (t.layerType == layerType)
            {
                tilemap = t.tilemap;
                return true;
            }
        }
        tilemap = null;
        return false;
    }

    public bool IsOccupied(Vector3Int cellPos)
    {
        return _worldTiles.TryGetValue(cellPos, out var st) && st.HasObstacle;
    }

    public Vector3Int ToTilePos(Vector3 worldPos)
    {
        if (GridConverter != null)
            return GridConverter.WorldToCell(worldPos);
        return new Vector3Int(Mathf.RoundToInt(worldPos.x), Mathf.RoundToInt(worldPos.y), 0);
    }

    // ----- Object placement -----

    public bool TryPlaceObject(Vector3Int cellPos, GameObject obj)
    {
        if (!_worldTiles.TryGetValue(cellPos, out var state))
            return false;

        if (state.HasPlacedObject)
            return false;

        state.PlacedObject = obj;

        return true;
    }

    public void RemoveObject(Vector3Int cellPos)
    {
        if (_worldTiles.TryGetValue(cellPos, out var state))
        {
            state.PlacedObject = null;
            state.WorldInteractable = null;

            if (state.tiles.Count == 0)
            {
                state.WorldInteractableType = ETileBlockType.None;
            }
            else
            {
                foreach (var kv in state.tiles)
                {
                    state.WorldInteractableType = kv.Value.WorldInteractableType;
                    break;
                }
            }
        }
    }

    public bool IsPlaceObject(Vector3Int cellPos)
    {
        return _worldTiles.TryGetValue(cellPos, out var st) && st.HasPlacedObject;
    }

    // ----- Tile removal -----

    public void RemoveTileState(Vector3Int pos)
    {
        if (_worldTiles.TryGetValue(pos, out var state))
        {
            state.tiles.Clear();
            state.WorldInteractable = null;

            if (!state.HasPlacedObject)
                _worldTiles.Remove(pos);
        }
    }

    public void UpdateTileInteractable(TileBaseDataState state)
    {
        state.WorldInteractable = _interactableFactory.SetStrategy(state.WorldInteractableType, state);
    }

    private readonly HashSet<TileObstacle> _drawnObstacles = new();
}