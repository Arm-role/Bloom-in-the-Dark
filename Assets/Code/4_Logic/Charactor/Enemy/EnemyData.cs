using System;
using UnityEngine;

public class EnemyData : CharacterData
{
    public Vector2 Position => _transform.position;
    private Transform _transform;

    public event Action<Vector2> OnMoveDirection;
    public event Action<Vector2> OnLookDirection;
    public event Action OnDied;

    public EnemyData(Transform transform)
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

    public void TakeDamage(float amount)
    {
        CurrentHP -= amount;
        if (CurrentHP <= 0)
        {
            CurrentHP = 0;
            OnDied?.Invoke();
        }
    }
}
