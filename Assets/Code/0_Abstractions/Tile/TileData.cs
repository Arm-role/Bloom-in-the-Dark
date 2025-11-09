using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileData
{
    public Vector2Int gridPosition;
    public ETileType baseType;

    // ชั้นของ tile เช่น overlay หรือ object ที่ซ้อนอยู่
    private Dictionary<ETileLayerType, IPlaceable> layers = new Dictionary<ETileLayerType, IPlaceable>();

    public bool HasLayer(ETileLayerType type) => layers.ContainsKey(type);

    public IPlaceable GetLayer(ETileLayerType type)
    {
        layers.TryGetValue(type, out IPlaceable placeable);
        return placeable;
    }

    public bool TryPlace(ETileLayerType layerType, IPlaceable item)
    {
        if (!item.CanPlaceOn(this, layerType))
            return false;

        layers[layerType] = item;
        return true;
    }

    public void RemoveLayer(ETileLayerType type)
    {
        if (layers.ContainsKey(type))
            layers.Remove(type);
    }
}