using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public readonly struct GridBaseData : IDataProvider
{
    public readonly List<TileInfo> PlacementInfos;
    private readonly Vector2? _pointerPosition;
    public Vector2? PointerPosition => _pointerPosition;

    public bool IsValid =>
        PlacementInfos != null &&
        PointerPosition.HasValue;

    public GridBaseData(
        List<TileInfo> placementInfos = null,
        Vector2? pointerPosition = null)
    {
        PlacementInfos = placementInfos;
        _pointerPosition = pointerPosition;
    }
}
