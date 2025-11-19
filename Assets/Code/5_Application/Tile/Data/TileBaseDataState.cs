using UnityEngine;
using System.Collections.Generic;

public class TileBaseDataState
{
    public Vector3Int CellPos { get; }
    public Vector3 WorldCenter { get; }

    public Dictionary<ETileLayerType, TileBaseData> tiles = new();

    public IPoolable<GameObject> PlacedObject { get; set; }
    public bool IsOccupied => PlacedObject != null && PlacedObject.IsAlive;

    public IWorldInteractable WorldInteractable { get; set; }
    public EWorldInteractableType WorldInteractableType { get; set; } = EWorldInteractableType.None;

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
        EWorldInteractableType selectedType = EWorldInteractableType.None;
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

        if (PlacedObject != null && PlacedObject.IsAlive && PlacedObject is WorldInteractable objInteract)
        {
            WorldInteractableType = objInteract.Type;
            return;
        }

        Debug.Log(selectedType);
        WorldInteractableType = selectedType;
    }
}