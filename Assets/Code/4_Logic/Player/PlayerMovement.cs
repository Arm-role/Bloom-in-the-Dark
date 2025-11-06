using UnityEngine;

public class PlayerMovement
{
    private readonly float _speed;

    public PlayerMovement(float speed)
    {
        _speed = speed;
    }
    public Vector2 CalculateVelocity(Vector2 direction)
    {
        return direction * _speed;
    }
}
