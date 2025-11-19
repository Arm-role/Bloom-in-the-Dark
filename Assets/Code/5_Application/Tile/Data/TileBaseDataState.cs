using UnityEngine;
using System.Collections.Generic;

public class TileBaseDataState
{
    public Vector3Int cellPos;
    public Vector3 WorldPosition { get; set; }

    public Dictionary<ETileLayerType, TileBaseData> tiles = new();

    public GameObject placedObject { get; set; }
    public bool IsOccupied => placedObject != null;


    public ETileStateFlags StateFlags;
    public IWorldInteractable WorldInteractable { get; set; }
    public EWorldInteractableType InteractableType { get; set; }


    public TileBaseDataState(Vector3Int pos)
    {
        cellPos = pos;
    }

    public TileBaseData GetTile(ETileLayerType layer)
    {
        tiles.TryGetValue(layer, out var tile);
        return tile;
    }

    public void SetTile(ETileLayerType layer, TileBaseData tile)
    {
        if (tiles.ContainsKey(layer)) tiles[layer] = tile;
        else tiles.Add(layer, tile);
    }
}