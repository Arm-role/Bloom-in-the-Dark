using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Linq;

public class WorldTileManager : MonoBehaviour
{
    // -----------------------------
    // Dependencies
    // -----------------------------
    private TileLibrary _tileLibrary;
    private ICellActionResolver _actionResolver;
    private TilemapRenderer _renderer;
    public GridConverter GridConverter { get; private set; }

    // -----------------------------
    // World Data
    // -----------------------------
    private Dictionary<Vector3Int, WorldCell> _cells = new();

    // -----------------------------
    // Initialization
    // -----------------------------
    public void Initialize(
        List<TilemapLayer> tilemapLayers,
        TileLibrary tileLibrary,
        GridConverter gridConverter,
        ICellActionResolver actionResolver)
    {
        _tileLibrary = tileLibrary;
        GridConverter = gridConverter;
        _actionResolver = actionResolver;
        _renderer = new TilemapRenderer(tilemapLayers);
        
        _cells.Clear();

        foreach (var layer in tilemapLayers)
        {
            if (layer.tilemap == null) continue;
            ScanTileLayer(layer.layerType, layer.tilemap);
        }

        ScanObstacles();
        Debug.Log($"✅ WorldTileManager initialized with {_cells.Count} tiles");
    }

    private void ScanTileLayer(ETileLayerType layerType, Tilemap tilemap)
    {
        var bounds = tilemap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                var cellPos = new Vector3Int(x, y, 0);
                var tile = tilemap.GetTile(cellPos);
                if (tile == null)
                    continue;

                var cell = GetOrCreateCell(cellPos);
                var tileData = _tileLibrary.GetTileData(tile);
                cell.AddTile(layerType, tileData);
            }
        }

        Debug.Log("ScanComplete");
        TileDomainEvents.TileScanCompleted();
    }

    // -----------------------------
    // Obstacle Scan
    // -----------------------------
    public void ScanObstacles()
    {
        foreach (var cell in _cells.Values)
            cell.ObstacleObject = null;

        TileObstacle[] obstacles = FindObjectsOfType<TileObstacle>();

        foreach (var ob in obstacles)
        {
            float cellSize = GridConverter.CellSize;
            Vector3 obstacleBL = ob.GetObstacleBottomLeft(cellSize);
            Vector3Int baseCell = GridConverter.WorldToCell(obstacleBL);

            for (int x = 0; x < ob.ObstacleSize.x; x++)
            {
                for (int y = 0; y < ob.ObstacleSize.y; y++)
                {
                    Vector3Int cell = new(baseCell.x + x, baseCell.y + y, 0);

                    if (_cells.TryGetValue(cell, out var state))
                    {
                        state.ObstacleObject = ob;
                    }
                }
            }
        }

        Debug.Log("📦 Obstacle scan complete. Count = " + obstacles.Length);

        TileDomainEvents.ObstacleScanCompleted();
    }

    public WorldCell GetCell(Vector3Int cellPos)
    {
        _cells.TryGetValue(cellPos, out var cell);
        return cell;
    }

    public WorldCell GetCellFromWorld(Vector3 worldPos)
    {
        var cellPos = GridConverter.WorldToCell(worldPos);
        return GetCell(cellPos);
    }

    public IEnumerable<WorldCell> GetAllCells()
        => _cells.Values;

    private WorldCell GetOrCreateCell(Vector3Int cellPos)
    {
        if (_cells.TryGetValue(cellPos, out var cell))
            return cell;

        var worldCenter = GridConverter.GetCellCenterWorld(cellPos);

        cell = new WorldCell(
            cellPos,
            worldCenter,
            _actionResolver);

        _cells.Add(cellPos, cell);
        return cell;
    }

    // -----------------------------
    // Runtime Tile Modification
    // -----------------------------
    public bool TryAddTile(
        Vector3Int cellPos,
        ETileLayerType layer,
        IBaseTileData tileData)
    {
        var cell = GetOrCreateCell(cellPos);

        if (!cell.AddTile(layer, tileData))
            return false;

        _renderer.SetTile(
            cellPos,
            layer, 
            tileData.Tiles.FirstOrDefault());
        
        return true;
    }

    public bool TryRemoveTile(
        Vector3Int cellPos,
        ETileLayerType layer)
    {
        if (!_cells.TryGetValue(cellPos, out var cell))
            return false;

        if (!cell.RemoveTile(layer))
            return false;

        if (cell.IsEmpty)
            _cells.Remove(cellPos);

        _renderer.ClearTile(cellPos, layer);
        
        return true;
    }

    // -----------------------------
    // Object Placement
    // -----------------------------
    public bool TryPlaceObject(Vector3Int cellPos, GameObject obj)
    {
        var cell = GetOrCreateCell(cellPos);

        if (!cell.PlaceObject(obj))
            return false;

        return true;
    }

    public void RemoveObject(Vector3Int cellPos)
    {
        if (!_cells.TryGetValue(cellPos, out var cell))
            return;

        cell.RemoveObject();

        if (cell.IsEmpty)
            _cells.Remove(cellPos);
    }

    // -----------------------------
    // Utility
    // -----------------------------

    public IReadOnlyList<WorldCell> GetCellsInRadius(
        Vector2 worldCenter,
        float radius)
    {
        List<WorldCell> result = new();

        float radiusSqr = radius * radius;

        Vector3Int minCell = GridConverter.WorldToCell(
            worldCenter - Vector2.one * radius);
        Vector3Int maxCell = GridConverter.WorldToCell(
            worldCenter + Vector2.one * radius);

        for (int x = minCell.x; x <= maxCell.x; x++)
        {
            for (int y = minCell.y; y <= maxCell.y; y++)
            {
                Vector3Int cellPos = new(x, y, 0);

                if (!_cells.TryGetValue(cellPos, out var cell))
                    continue;

                // เช็คระยะจริงจาก center ของ cell
                float distSqr =
                    (cell.WorldCenter - (Vector3)worldCenter).sqrMagnitude;

                if (distSqr <= radiusSqr)
                    result.Add(cell);
            }
        }

        return result;
    }

    public IReadOnlyList<WorldCell> GetCellsAlongLine(
        Vector2 origin,
        Vector2 dir,
        float length)
    {
        List<WorldCell> result = new();

        dir.Normalize();

        float step = GridConverter.CellSize * 0.5f;
        float traveled = 0f;

        HashSet<Vector3Int> visited = new();

        while (traveled <= length)
        {
            Vector2 worldPos = origin + dir * traveled;
            Vector3Int cellPos = GridConverter.WorldToCell(worldPos);

            if (!visited.Contains(cellPos))
            {
                visited.Add(cellPos);

                if (_cells.TryGetValue(cellPos, out var cell))
                    result.Add(cell);
            }

            traveled += step;
        }

        return result;
    }

    public IReadOnlyList<WorldCell> GetCellsInLine(
        Vector2 origin, 
        Vector2 dir, 
        float length,
        float width)
    {
        List<WorldCell> result = new();
        HashSet<Vector3Int> visited = new();

        dir.Normalize();
        Vector2 right = new Vector2(-dir.y, dir.x); 

        float halfWidth = width * 0.5f;
        float cellSize = GridConverter.CellSize;

        Vector2 end = origin + dir * length;

        Vector2 min = Vector2.Min(origin, end) - Vector2.one * halfWidth;
        Vector2 max = Vector2.Max(origin, end) + Vector2.one * halfWidth;

        Vector3Int minCell = GridConverter.WorldToCell(min);
        Vector3Int maxCell = GridConverter.WorldToCell(max);

        for (int x = minCell.x; x <= maxCell.x; x++)
        {
            for (int y = minCell.y; y <= maxCell.y; y++)
            {
                Vector3Int cellPos = new(x, y, 0);

                if (!_cells.TryGetValue(cellPos, out var cell))
                    continue;

                if (!visited.Add(cellPos))
                    continue;

                Vector2 toCell = (Vector2)cell.WorldCenter - origin;

                // --- ระยะตามแนวเส้น ---
                float forward = Vector2.Dot(toCell, dir);
                if (forward < 0f || forward > length)
                    continue;

                // --- ระยะออกด้านข้าง ---
                float side = Mathf.Abs(Vector2.Dot(toCell, right));
                if (side > halfWidth + cellSize * 0.5f)
                    continue;

                result.Add(cell);
            }
        }

        return result;
    }
    public IReadOnlyList<WorldCell> GetCellsFromArea(
        Vector2 origin,
        Vector2 size)
    {
        List<WorldCell> result = new();

        // half extents
        Vector2 half = size * 0.5f;

        // world bounds
        Vector2 minWorld = origin - half;
        Vector2 maxWorld = origin + half;

        // convert to cell bounds
        Vector3Int minCell = GridConverter.WorldToCell(minWorld);
        Vector3Int maxCell = GridConverter.WorldToCell(maxWorld);

        for (int x = minCell.x; x <= maxCell.x; x++)
        {
            for (int y = minCell.y; y <= maxCell.y; y++)
            {
                Vector3Int cellPos = new(x, y, 0);

                if (_cells.TryGetValue(cellPos, out var cell))
                {
                    result.Add(cell);
                }
            }
        }

        return result;
    }
}