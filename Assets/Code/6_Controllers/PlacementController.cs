using System.Collections.Generic;
using UnityEngine;

public class PlacementController : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private Vector3 _gridOrigin = Vector3.zero;
    [SerializeField] private float cellSize = 1;
    [SerializeField] private Vector2Int size = Vector2Int.one;

    // --- Mock Data (ในเกมจริงจะมาจากระบบ Inventory) ---
    //[SerializeField] private PlaceableItemData itemToPlace;

    private IPlayerInput _playerInput;

    // --- Logic & Dependencies ---
    private GridLogic _gridLogic;
    private WorldGridLogic _worldGridLogic;

    private PlacementPreviewController _previewController;

    private void Awake()
    {
        _gridLogic = new GridLogic(cellSize, _gridOrigin);
        _worldGridLogic = new WorldGridLogic();
    }
    public void Initialze(IPlayerInput playerInput, PlacementPreviewController previewController)
    {
        _playerInput = playerInput;
        _previewController = previewController;
        _previewController.Hide();
    }

    private void Update()
    {
        List<PreviewTileInfo> previewInfos = CalculatePreviewInfos();
        _previewController.UpdatePreview(previewInfos);

        if (_playerInput == null) return;

        if (_playerInput.IsSecorndaryActionDown)
        {
            HandlePlacementClick();
        }
    }

    private void HandlePlacementClick()
    {
        List<PreviewTileInfo> placementInfos = CalculatePreviewInfos();

        bool canPlaceOverall = true;
        foreach (var info in placementInfos)
        {
            if (!info.CanPlace)
            {
                canPlaceOverall = false;
                break;
            }
        }

        if (canPlaceOverall)
        {
            _worldGridLogic.PlaceObjectAt(placementInfos);
        }
    }

    private List<PreviewTileInfo> CalculatePreviewInfos()
    {
        Vector3 mouseWorldPos = _playerInput.PointerWorldPosition;
        Vector2Int mouseGridPos = _gridLogic.WorldToGrid(mouseWorldPos);

        Vector2Int originGridPos = CalculateOriginGridPos(mouseGridPos, size);

        bool[,] validityMap = _worldGridLogic.GetPlacementValidity(originGridPos, size, _gridLogic.GridToWorld);

        var tileInfos = new List<PreviewTileInfo>();
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector2Int tileGridPos = new Vector2Int(originGridPos.x + x, originGridPos.y + y);

                tileInfos.Add(new PreviewTileInfo
                {
                    WorldPosition = _gridLogic.GridToWorld(tileGridPos),
                    CanPlace = validityMap[x, y]
                });
            }
        }

        return tileInfos;
    }
    private Vector2Int CalculateOriginGridPos(Vector2Int anchorPos, Vector2Int itemSize)
    {
        return anchorPos - new Vector2Int(
            Mathf.FloorToInt(itemSize.x / 2f),
            Mathf.FloorToInt(itemSize.y / 2f));
    }
}
