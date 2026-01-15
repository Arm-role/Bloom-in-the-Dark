using UnityEngine;

public class ProximityIndicator
{
    private float _range;
    private Vector2 _playerPosition;

    public float Range => _range;
    public Vector2 PlayerPosition => _playerPosition;

    public void Setup(float range)
    {
        _range = range;
    }

    public void UpdatePlayerPosition(Vector2 pos)
    {
        _playerPosition = pos;
    }

    public bool IsInsideRange(Vector2 point)
    {
        return Vector2.Distance(point, _playerPosition) <= _range;
    }
}
