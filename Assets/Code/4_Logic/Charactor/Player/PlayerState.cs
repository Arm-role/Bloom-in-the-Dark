using System;
using UnityEngine;

public class PlayerState : CharacterState, IPlayerState
{
    public Vector2 MoveDirection { get; private set; } 
    public Vector2 LookDirection { get; private set; }
    public FacingDirection Facing { get; private set; }
    
    public bool IsMoving => base.MoveDirection.sqrMagnitude > 0.01f;

    public Action<Vector2> OnMoveDirection;
    public Action<Vector2> OnLookDirection;
    public Action<Vector2> OnDirectionChanged;
    public PlayerState(FacingDirection facing)
    {
        Facing = facing;
        MoveDirection = FacingToVector(facing);
    }
    
    public void UpdateMoveDirection(Vector2 input)
    {
        OnMoveDirection?.Invoke(input);
        base.MoveDirection = input;

        if (input != Vector2.zero)
        {
            LookDirection = input.normalized;
            MoveDirection = input.normalized;

            UpdateFacingByVector(input);
            OnLookDirection?.Invoke(LookDirection);
            OnDirectionChanged?.Invoke(MoveDirection);
        }
        else
        {
            UpdateDirectionByLook();
        }
    }

    public void Look(Vector2 lookDir)
    {
        if (lookDir == Vector2.zero)
            return;

        LookDirection = lookDir.normalized;
        UpdateFacingByVector(lookDir);
        UpdateDirectionByLook();

        OnLookDirection?.Invoke(LookDirection);
    }

    private void UpdateDirectionByLook()
    {
        if (!IsMoving && LookDirection != Vector2.zero)
        {
            MoveDirection = LookDirection.normalized;
            OnDirectionChanged?.Invoke(MoveDirection);
        }
    }

    private void UpdateFacingByVector(Vector2 dir)
    {
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            Facing = dir.x > 0 ? FacingDirection.Right : FacingDirection.Left;
        else
            Facing = dir.y > 0 ? FacingDirection.Up : FacingDirection.Down;
    }

    private  Vector2 FacingToVector(FacingDirection facing)
    {
        return facing switch
        {
            FacingDirection.Right => Vector2.right,
            FacingDirection.Left => Vector2.left,
            FacingDirection.Up => Vector2.up,
            _ => Vector2.down
        };
    }

    public void SetInteractionCooldown(EInteractionIntentType feedbackIntentType, float feedbackCooldown)
    {
    }
}
