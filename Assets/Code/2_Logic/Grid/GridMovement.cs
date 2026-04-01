using UnityEngine;

public class GridMovement
{
    private readonly GridLogic _gridLogic;
    private Vector2Int _currentGridPosition;

    public Vector2Int CurrentGridPosition => _currentGridPosition;

    public GridMovement(GridLogic gridLogic, Vector3 startingWorldPosition)
    {
        _gridLogic = gridLogic;
        _currentGridPosition = _gridLogic.WorldToGrid(startingWorldPosition);
    }

    public Vector3 GetNextWorldPosition(Vector2Int direction)
    {
        Vector2Int targetGridPosition = _currentGridPosition + direction;

        _currentGridPosition = targetGridPosition;

        return _gridLogic.GridToWorld(targetGridPosition);
    }
}