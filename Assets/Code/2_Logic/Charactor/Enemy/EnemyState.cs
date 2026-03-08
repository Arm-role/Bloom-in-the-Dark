using System;
using UnityEngine;

public class EnemyState : CharacterState
{
  public Vector2 Position => _transform.position;
  private Transform _transform;

  public event Action<Vector2> OnMoveDirection;
  public event Action<Vector2> OnLookDirection;

  public EnemyState(Transform transform)
  {
    _transform = transform;
  }

  public void SetMoveDirection(Vector2 dir)
  {
    MoveDirection = dir;
    if (dir != Vector2.zero)
    {
      LookDirection = dir.normalized;
      OnLookDirection?.Invoke(LookDirection);
    }
    OnMoveDirection?.Invoke(MoveDirection);
  }

  public void SetLookDirection(Vector2 dir)
  {
    if (dir != Vector2.zero)
    {
      LookDirection = dir.normalized;
      OnLookDirection?.Invoke(LookDirection);
    }
  }
}
