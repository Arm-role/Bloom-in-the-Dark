using System.Collections;
using UnityEngine;

public class AreaCircleIndicator
{
    private readonly float _xAngle;
    private readonly float _yScale;

    private float _range;
    private float _areaRadius;

    private Vector2 _playerPosition;
    private Vector2 _areaRadiusPosition;

    public Vector2 PlayerPosition => _playerPosition;
    public Vector2 AreaRadiusPositionPosition => _areaRadiusPosition;
    public float Range => _range;
    public float YScale => _yScale;

    public AreaCircleIndicator(float xAngle, float range, float areaRadius)
    {
        _xAngle = xAngle;
        _range = range;
        _areaRadius = areaRadius;
        _yScale = Mathf.Cos(_xAngle * Mathf.Deg2Rad);
    }
    public void Setup(float range, float areaRadius)
    {
        _range = range;
        _areaRadius = areaRadius;
    }

    public void UpdatePlayerPosition(Vector2 newPos) => _playerPosition = newPos;

    public (Vector2 rangePos, Vector2 healPos, Quaternion rotation, Vector3 rangeScale, Vector3 healScale)
        CalculatePreview(Vector2 pointerWorld)
    {
        Vector2 dir = pointerWorld - _playerPosition;
        Vector2 ellipseDir = new Vector2(dir.x, dir.y / _yScale);
        float distance = Mathf.Min(ellipseDir.magnitude, _range);

        _areaRadiusPosition = _playerPosition + new Vector2(
            ellipseDir.normalized.x * distance,
            ellipseDir.normalized.y * distance * _yScale
        );

        Quaternion flatRotation = Quaternion.Euler(_xAngle, 0f, 0f);
        Vector3 rangeScale = new Vector3(_range * 2f, _range * 2f);
        Vector3 healScale = new Vector3(_areaRadius * 2f, _areaRadius * 2f);

        return (_playerPosition, _areaRadiusPosition, flatRotation, rangeScale, healScale);
    }

    public bool IsInsideRange(Vector2 point)
    {
        Vector2 local = point - _playerPosition;
        float x = local.x;
        float y = local.y / _yScale;
        float value = (x * x + y * y);
        return value <= _range * _range;
    }

    public bool IsInsideHeal(Vector2 point)
    {
        Vector2 local = point - _areaRadiusPosition;
        float x = local.x;
        float y = local.y / _yScale;
        float value = (x * x + y * y);
        return value <= _areaRadius * _areaRadius;
    }

    public (Vector2 playerPos, Vector2 healPos, float rangeRadius, float healRadius, float yScale)
        GetEllipseData()
    {
        return (_playerPosition, _areaRadiusPosition, _range, _areaRadius, _yScale);
    }
}
