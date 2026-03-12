
using System.Collections.Generic;
using UnityEngine;

public class PlacementPreviewController : MonoBehaviour, IPlacementPreview
{
    [Header("Dependencies")]
    [SerializeField] private GameObject tilePrefab;

    [Header("Settings")]
    [SerializeField] private Color canPlaceColor = new Color(0, 1, 0, 0.5f);
    [SerializeField] private Color cannotPlaceColor = new Color(1, 0, 0, 0.5f);
    [SerializeField] private Color outOfRangeColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

    private readonly List<PreviewGridView> _tilePool = new List<PreviewGridView>();
    private PreviewGridView _view;

    public GameObject GameObject => gameObject;

    public void Initialze(PreviewGridView view)
    { 
        _view = view;
        _view.HideAllTiles();
    }

    // ✅ สำหรับ grid-based (เช่น placement)
    public void UpdatePreview(List<TileInfo> tilesToDisplay)
    {
        if (tilesToDisplay == null || tilesToDisplay.Count == 0)
        {
            Hide();
            return;
        }

        int requiredTiles = tilesToDisplay.Count;
        EnsureTilePool(requiredTiles);
        _view.HideAllTiles();

        for (int i = 0; i < requiredTiles; i++)
        {
            var info = tilesToDisplay[i];
            var color = GetColorForState(info.State);
            _view.UpdateTile(i, info.WorldPosition, color);
        }
    }

    public void Hide()
    {
        _view.HideAllTiles();
    }

    private void EnsureTilePool(int required)
    {
        while (_tilePool.Count < required)
        {
            var newTile = Instantiate(tilePrefab, transform);
            var gridView = newTile.GetComponent<PreviewGridView>();
            _tilePool.Add(gridView);
            _view.AddTile(newTile);
        }
    }

    private Color GetColorForState(PlacementState state) => state switch
    {
        PlacementState.Valid => canPlaceColor,
        PlacementState.Blocked => cannotPlaceColor,
        PlacementState.OutOfRange => outOfRangeColor,
        _ => Color.white
    };
}