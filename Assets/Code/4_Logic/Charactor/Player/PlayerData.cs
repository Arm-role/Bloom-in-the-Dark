using System;
using UnityEngine;

public class PlayerData : CharacterData, IPlayerData
{
    public Vector2 Direction { get; private set; }      // ใช้เป็นทิศทางปัจจุบัน (ดู+เดิน)

    public PlayerData(FacingDirection facing)
    {
        Facing = facing;
    }

    public bool IsMoving => MoveDirection.sqrMagnitude > 0.01f;

    public Action<Vector2> OnMoveDirection;
    public Action<Vector2> OnLookDirection;
    public Action<Vector2> OnDirectionChanged;

    public void UpdateMoveDirection(Vector2 input)
    {
        OnMoveDirection?.Invoke(input);
        MoveDirection = input;

        if (input != Vector2.zero)
        {
            LookDirection = input.normalized;
            Direction = input.normalized;

            UpdateFacingByVector(input);

            OnLookDirection?.Invoke(LookDirection);
            OnDirectionChanged?.Invoke(Direction);
        }
        else
        {
            UpdateDirectionByLook();
        }
    }

    public void Look(Vector2 lookDir)
    {
        OnLookDirection?.Invoke(lookDir);
        LookDirection = lookDir;

        if (lookDir != Vector2.zero)
        {
            UpdateFacingByVector(lookDir);
            UpdateDirectionByLook();
        }
    }

    private void UpdateDirectionByLook()
    {
        if (!IsMoving && LookDirection != Vector2.zero)
        {
            Direction = LookDirection.normalized;
            OnDirectionChanged?.Invoke(Direction);
        }
    }

    private void UpdateFacingByVector(Vector2 dir)
    {
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            Facing = dir.x > 0 ? FacingDirection.Right : FacingDirection.Left;
        else
            Facing = dir.y > 0 ? FacingDirection.Up : FacingDirection.Down;
    }
}
