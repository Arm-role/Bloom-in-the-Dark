using System.Collections.Generic;
using UnityEngine;

public interface IWorldCell
{
    Vector3Int CellPos { get; }
    Vector3 WorldCenter { get; }

    public bool IsWatered { get; }

    public GameObject PlacedObject { get; }

    bool HasObstacle { get; }
    bool HasPlacedObject { get; }
    bool HasAnyInteractable { get; }

    bool AddTile(
        ETileLayerType layer,
        IBaseTileData tileData);

    bool RemoveTile(ETileLayerType layer);
    bool PlaceObject(GameObject obj);
    void RemoveObject();
    IBaseTileData GetUpperTile();
}