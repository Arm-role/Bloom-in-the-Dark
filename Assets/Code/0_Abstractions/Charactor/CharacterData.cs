using UnityEngine;

public abstract class CharacterData
{
    public Vector2 MoveDirection { get; protected set; }
    public Vector2 LookDirection { get; protected set; }
    public FacingDirection Facing { get; protected set; }
    public float MoveSpeed { get; set; }
    public float AttackRange { get; set; }
    public float MaxHP { get; set; }
    public float CurrentHP { get; set; }

    public bool IsDead => CurrentHP <= 0;
}