using System.Collections.Generic;
using UnityEngine;

public interface IPlacementPreview
{
    public GameObject GameObject { get; }
    public void UpdatePreview(List<TileInfo> tilesToDisplay);
    public void Hide();
}