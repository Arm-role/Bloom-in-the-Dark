using UnityEngine;
using System.Collections.Generic;

public class TileBaseDataState
{
    public Vector3Int CellPos { get; }
    public Vector3 WorldCenter { get; }

    public Dictionary<ETileLayerType, TileBaseData> tiles = new();

    public GameObject PlacedObject { get; set; }
    public TileObstacle ObstacleObject { get; set; }
    public bool HasObstacle
    {
        get
        {
            if (ObstacleObject != null)
            {
                return true;
            }

            if (WorldInteractableType == ETileBlockType.Blocked)
            {
                return true;
            }

            return false;
        }
    }

    public bool HasPlacedObject
    {
        get
        {
            if (PlacedObject == null) return false;

            var poolable = PlacedObject.GetComponent<IPoolable<GameObject>>();
            return poolable != null && poolable.IsAlive;
        }
    }

    public IWorldInteractable WorldInteractable { get; set; }
    public ETileBlockType WorldInteractableType { get; set; } = ETileBlockType.None;

    public TileBaseDataState(Vector3Int cellPos, Vector3 worldCenter)
    {
        CellPos = cellPos;
        WorldCenter = worldCenter;
    }

    public TileBaseData GetTile(ETileLayerType layer)
    {
        tiles.TryGetValue(layer, out var tile);
        return tile;
    }

    public void SetTile(ETileLayerType layer, TileBaseData tile)
    {
        if (tile != null)
        {
            tiles[layer] = tile;
        }
        else
        {
            tiles.Remove(layer);
        }

        UpdateWorldInteractableType();
    }

    public void UpdateWorldInteractableType()
    {
        ETileBlockType selectedType = ETileBlockType.None;
        int highestPriority = -1;

        foreach (var kv in tiles)
        {
            int priority = TileLayerPriority.GetPriority(kv.Key);
            if (priority > highestPriority)
            {
                highestPriority = priority;
                selectedType = kv.Value.WorldInteractableType;
            }
        }

        if (PlacedObject != null &&
            PlacedObject.GetComponent<IPoolable<GameObject>>().IsAlive &&
            PlacedObject.TryGetComponent<WorldInteractable>(out var objInteract))
        {
            WorldInteractableType = objInteract.Type;
            return;
        }

        WorldInteractableType = selectedType;
    }
}
