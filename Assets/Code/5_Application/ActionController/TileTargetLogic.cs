using System.Collections.Generic;
using UnityEngine;

public class TileTargetLogic
{
    private readonly GridConverter _gridConverter;

    private float _maxDistance;
    private Vector3 _playerPosition;
    private Vector3 _targetWorldPosition;
    private Vector3 _playerDirection;

    public Vector3 PlayerPosition => _playerPosition;
    public Vector3 TargetWorldPosition => _targetWorldPosition;
    public Vector3 PlayerDirection => _playerDirection;
    public float MaxDistance => _maxDistance;

    public TileTargetLogic(GridConverter gridConverter)
    {
        _gridConverter = gridConverter;
    }

    public void Setup(Vector3 playerPos, float maxDistance)
    {
        _playerPosition = playerPos;
        _maxDistance = maxDistance;
    }

    public void UpdateState(Vector3 newPlayerPos, Vector3 newDirection)
    {
        _playerPosition = newPlayerPos;
        _playerDirection = newDirection.normalized;
    }

    public Vector3 ClampTarget(Vector3 pointerPos)
    {
        Vector2Int gridPos = _gridConverter.WorldToGrid(pointerPos);
        Vector3 worldPos = _gridConverter.GridToWorld(gridPos);

        float dist = Vector3.Distance(_playerPosition, worldPos);
        if (dist > _maxDistance)
        {
            Vector3 dir = (worldPos - _playerPosition).normalized;
            Vector3 clamped = _playerPosition + dir * _maxDistance;

            Vector2Int clampedCell = _gridConverter.WorldToGrid(clamped);
            return _gridConverter.GridToWorld(clampedCell);
        }

        return worldPos;
    }

    public Vector3 ClampTarget(Vector3 pointerPos, Vector3 playerDirection)
    {
        Vector2Int gridPos = _gridConverter.WorldToGrid(pointerPos);
        Vector3 worldPos = _gridConverter.GridToWorld(gridPos);

        float dist = Vector3.Distance(_playerPosition, worldPos);
        if (dist > _maxDistance)
        {
            Vector3 forwardTileWorld = _playerPosition + playerDirection.normalized;
            Vector2Int frontCell = _gridConverter.WorldToGrid(forwardTileWorld);
            return _gridConverter.GridToWorld(frontCell);
        }

        return worldPos;
    }

    public (Vector3 playerPos, Vector3 targetPos, Vector3 rangeScale)
        CalculatePreview(Vector3 pointerPos)
    {
        _targetWorldPosition = ClampTarget(pointerPos, _playerDirection);

        Vector3 rangeScale = new Vector3(_maxDistance * 2f, _maxDistance * 2f, 1f);

        return (_playerPosition, _targetWorldPosition, rangeScale);
    }

    public bool IsInsideRange(Vector3 point)
    {
        return Vector3.Distance(_playerPosition, point) <= _maxDistance;
    }

    public List<TileInfo> GetTargetTilePreview(Vector3 pointerPos)
    {
        var (_, target, _) = CalculatePreview(pointerPos);

        return new List<TileInfo>
        {
            new TileInfo
            {
                WorldPosition = target,
                State = IsInsideRange(target)
                    ? PlacementState.Valid
                    : PlacementState.OutOfRange
            }
        };
    }
}