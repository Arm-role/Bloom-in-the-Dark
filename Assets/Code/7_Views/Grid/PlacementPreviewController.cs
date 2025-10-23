
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlacementPreviewController : MonoBehaviour, IPracementPreview
{
    [Header("Dependencies")]
    [SerializeField] private GameObject tilePrefab;

    [Header("Settings")]
    [SerializeField] private Color canPlaceColor = new Color(0, 1, 0, 0.5f);
    [SerializeField] private Color cannotPlaceColor = new Color(1, 0, 0, 0.5f);
    [SerializeField] private Color outOfRangeColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

    private List<PreviewGridView> _tilePool = new List<PreviewGridView>();
    private PreviewGridView view;

    public GameObject GameObject => gameObject;

    public void Initialze(PreviewGridView view)
    {
        this.view = view;
    }

    public void UpdatePreview(List<PreviewTileInfo> tilesToDisplay)
    {
        int requiredTiles = tilesToDisplay.Count;

        while (_tilePool.Count < requiredTiles)
        {
            var newTile = Instantiate(tilePrefab);

            _tilePool.Add(newTile.GetComponent<PreviewGridView>());
            view.AddTile(newTile);
        }

        view.HideAllTiles();

        for (int i = 0; i < requiredTiles; i++)
        {
            PreviewTileInfo info = tilesToDisplay[i];

            Color color = Color.white;

            switch(info.State)
            {
                case PlacementState.Valid: color = canPlaceColor; break;
                case PlacementState.Blocked: color = cannotPlaceColor; break;
                case PlacementState.OutOfRange: color = outOfRangeColor; break;
            }
            view.UpdateTile(i, info.WorldPosition, color);
        }
    }

    public void Hide()
    {
        view.HideAllTiles();
    }
}