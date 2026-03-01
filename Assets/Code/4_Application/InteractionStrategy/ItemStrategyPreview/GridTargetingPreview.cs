using System.Collections.Generic;
using UnityEngine;

public class GridTargetingPreview : IInteractionPreview
{
    private readonly IPlacementPreview _preview;

    public GridTargetingPreview(IPlacementPreview preview)
    {
        _preview = preview;
    }

    public void Setup()
    {
        _preview.Hide();
    }

    public void Update(TargetResult result)
    {
        if (!result.IsValid)
        {
            _preview.Hide();
            return;
        }

        Debug.Log("Update Preview");
        var tiles = BuildTileInfos(result.Cells);
        _preview.UpdatePreview(tiles);
    }

    public void Hide()
    {
        _preview.Hide();
    }
    
    private static List<TileInfo> BuildTileInfos(
        IReadOnlyList<WorldCell> cells)
    {
        var list = new List<TileInfo>(cells.Count);

        foreach (var cell in cells)
        {
            var state =
                cell.HasPlacedObject
                    ? PlacementState.Blocked
                    : PlacementState.Valid;

            list.Add(new TileInfo
            {
                WorldPosition = cell.WorldCenter,
                State = state
            });
        }

        return list;
    }
}