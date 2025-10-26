using System.Collections.Generic;
using UnityEngine;

public class PlacementController : MonoBehaviour
{
    private Vector2Int _size = Vector2Int.one;
    private Vector2 _playerPosition = Vector2Int.zero;
    private float _maxPlaceDistance = 2f;

    // --- Logic & Dependencies ---
    private bool _isActive = false;

    private GridLogic _gridLogic;
    private WorldGridLogic _worldGridLogic;

    private IPracementPreview _previewController;

    public void Initialze(IPracementPreview previewController, WorldGridLogic worldGridLogic)
    {
        _worldGridLogic = worldGridLogic;
        _previewController = previewController;
        _previewController.Hide();
    }

    public void Setup(Vector2Int gridSize, float maxPlaceDistance)
    {
        _size = gridSize;
        _maxPlaceDistance = maxPlaceDistance;

        _gridLogic = new GridLogic(1, new Vector3(0.5f, 0.5f, 0f));
    }
    public IDataProvider HandlePlacementClick(IItemInstance itemInstance, Vector2 playerPosition, Vector2 pointerPosition)
    {
        GridInteractionData data = new GridInteractionData();

        data.PlacementInfos = CalculatePreviewInfos(playerPosition, pointerPosition);
        bool canPlaceOverall = true;

        foreach (var info in data.PlacementInfos)
        {
            if (info.State != PlacementState.Valid)
            {
                canPlaceOverall = false;
                break;
            }
        }

        if (canPlaceOverall)
        {
            _worldGridLogic.PlaceObjectAt(data.PlacementInfos);
            return data;
        }

        return null;
    }

    public void EnablePreview(Vector2 playerPosition, Vector2 pointerPosition)
    {
        _isActive = true;
        _playerPosition = playerPosition;
        _previewController.GameObject.SetActive(true);
        UpdatePreview(playerPosition, pointerPosition);
    }

    public void UpdatePreview(Vector2 playerPosition, Vector2 pointerPosition)
    {
        if (!_isActive) return;
        _playerPosition = playerPosition;
        List<TileInfo> previewInfos = CalculatePreviewInfos(playerPosition, pointerPosition);
        _previewController.UpdatePreview(previewInfos);
    }

    public void DisablePreview()
    {
        _isActive = false;
        _previewController.Hide();
    }

    private List<TileInfo> CalculatePreviewInfos(Vector2 playerPosition, Vector2 pointerPosition)
    {
        Vector2Int mouseGridPos = _gridLogic.WorldToGrid(pointerPosition);
        Vector2Int originGridPos = CalculateOriginGridPos(mouseGridPos, _size);

        bool[,] validityMap = _worldGridLogic.GetPlacementValidity(originGridPos, _size, _gridLogic.GridToWorld);

        var tileInfos = new List<TileInfo>();

        for (int x = 0; x < _size.x; x++)
        {
            for (int y = 0; y < _size.y; y++)
            {
                Vector2Int tileGridPos = new Vector2Int(originGridPos.x + x, originGridPos.y + y);
                Vector3 worldPos = _gridLogic.GridToWorld(tileGridPos);

                float distance = Vector2.Distance(playerPosition, worldPos);

                PlacementState state;

                if (distance > _maxPlaceDistance)
                    state = PlacementState.OutOfRange;
                else if (!validityMap[x, y])
                    state = PlacementState.Blocked;
                else
                    state = PlacementState.Valid;

                tileInfos.Add(new TileInfo
                {
                    WorldPosition = worldPos,
                    State = state
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

    private void OnDrawGizmos()
    {
        Vector2 center = _playerPosition;
        float radius = _maxPlaceDistance;
        int circleSegments = 64;

        Gizmos.color = Color.green;

        DrawCircle(center, radius, circleSegments);
    }

    private void DrawCircle(Vector2 center, float radius, int segments)
    {
        if (radius <= 0f) return;

        float angleStep = 2f * Mathf.PI / segments;
        Vector3 prevPoint = new Vector3(
            center.x + Mathf.Cos(0) * radius,
            center.y + Mathf.Sin(0) * radius,
            0f
        );

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep;
            Vector3 nextPoint = new Vector3(
                center.x + Mathf.Cos(angle) * radius,
                center.y + Mathf.Sin(angle) * radius,
                0f
            );
            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }
    }
}