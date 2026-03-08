using UnityEngine;

public class AreaCircleShape
{
    private float _xAngle;
    private float _yScale;

    private float _range;
    private float _areaRadius;

    public float Range => _range;
    public float AreaRadius => _areaRadius;
    public float YScale => _yScale;

    public void Setup(float xAngle, float range, float areaRadius)
    {
        _xAngle = xAngle;
        _yScale = Mathf.Cos(_xAngle * Mathf.Deg2Rad);

        _range = range;
        _areaRadius = Mathf.Max(0.1f, areaRadius);
    }

    // -------------------------------------------------
    // Geometry
    // -------------------------------------------------

    public Vector2 Clamp(Vector2 origin, Vector2 pointer)
    {
        Vector2 dir = pointer - origin;
        Vector2 ellipseDir = new Vector2(dir.x, dir.y / _yScale);

        float dist = Mathf.Min(ellipseDir.magnitude, _range);

        return origin + new Vector2(
            ellipseDir.normalized.x * dist,
            ellipseDir.normalized.y * dist * _yScale
        );
    }

    public bool IsInsideRange(Vector2 origin, Vector2 point)
    {
        Vector2 local = point - origin;
        float x = local.x;
        float y = local.y / _yScale;
        return (x * x + y * y) <= _range * _range;
    }

    public bool IsInsideArea(Vector2 center, Vector2 point)
    {
        Vector2 local = point - center;
        float x = local.x;
        float y = local.y / _yScale;
        return (x * x + y * y) <= _areaRadius * _areaRadius;
    }

    // -------------------------------------------------
    // Preview helper
    // -------------------------------------------------

    public AreaCirclePreviewData GetPreview(
        Vector2 origin,
        Vector2 pointer)
    {
        Vector2 center = Clamp(origin, pointer);

        Vector3 rangeScale =
            new Vector3(_range * 2f, _range * 2f * _yScale, 1f);

        Vector3 areaScale =
            new Vector3(_areaRadius * 2f, _areaRadius * 2f * _yScale, 1f);

        return new AreaCirclePreviewData(
            origin,
            center,
            rangeScale,
            areaScale
        );
    }
}
