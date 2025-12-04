using System;
using UnityEngine;

public class EnemyData
{
    public Vector2 Position => _transform.position;
    private Transform _transform;

    public Vector2 MoveDirection { get; private set; }
    public Vector2 LookDirection { get; private set; }
    public float MoveSpeed { get; set; }
    public float AttackRange { get; set; }
    public int MaxHP { get; set; }
    public int CurrentHP { get; set; }

    public bool IsDead => CurrentHP <= 0;

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

    public void TakeDamage(int amount)
    {
        CurrentHP -= amount;
        if (CurrentHP <= 0)
        {
            CurrentHP = 0;
            OnDied?.Invoke();
        }
    }
}
