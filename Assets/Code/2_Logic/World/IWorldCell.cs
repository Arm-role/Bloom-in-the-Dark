using UnityEngine;

public interface IWorldCell
{
  Vector3Int CellPos { get; }
  Vector3 WorldCenter { get; }

  public bool IsWatered { get; }

  public GameObject Object { get; }

  bool BlocksMovement { get; }
  bool HasPlacedObject { get; }
  bool HasAnyInteractable { get; }

  bool AddTile(
      ETileLayerType layer,
      IBaseTileData tileData);

  bool RemoveTile(ETileLayerType layer);
  bool PlaceObject(GameObject obj, CellObjectFlags flags);
  void RemoveObject();
  IBaseTileData GetUpperTile();
  bool HasTile<T>() where T : IBaseTileData;
}
