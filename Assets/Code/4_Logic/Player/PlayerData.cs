using System;
using UnityEngine;

public class PlayerData : IPlayerData
{
    public Vector2 MoveDirection { get; private set; }
    public FacingDirection Facing { get; private set; } = FacingDirection.Right;

    public bool IsMoving => MoveDirection.sqrMagnitude > 0.01f;

    public Action<Vector2> OnMoveDirection;
    public Action<Vector2> OnLookDirection;

    public void UpdateMoveDirection(Vector2 input)
    {
        OnMoveDirection?.Invoke(input);

        if (input == Vector2.zero) return;

        MoveDirection = input;
        UpdateFacingByVector(input);
    }

    public void Look(Vector2 lookDir)
    {
        if (lookDir == Vector2.zero) return;
        OnLookDirection?.Invoke(lookDir);
        UpdateFacingByVector(lookDir);
    }

    private void UpdateFacingByVector(Vector2 dir)
    {
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            Facing = dir.x > 0 ? FacingDirection.Right : FacingDirection.Left;
        else
            Facing = dir.y > 0 ? FacingDirection.Up : FacingDirection.Down;
    }
}