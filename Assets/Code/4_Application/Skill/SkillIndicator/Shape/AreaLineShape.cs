using UnityEngine;

public class AreaLineShape
{
    private float _yScale;
    private float _length;
    private float _halfWidth;

    public float Length => _length;
    public float Width => _halfWidth * 2f;

    public void Setup(
        float xAngle,
        float length,
        float width)
    {
        _yScale = Mathf.Cos(xAngle * Mathf.Deg2Rad);
        _length = Mathf.Max(0.1f, length);
        _halfWidth = Mathf.Max(0.05f, width * 0.5f);
    }

    // ---------------------------------------------
    // Geometry
    // ---------------------------------------------

    public Vector2 ClampEnd(
        Vector2 origin,
        Vector2 pointer)
    {
        Vector2 dir = pointer - origin;
        Vector2 ellipseDir = new(dir.x, dir.y / _yScale);

        if (ellipseDir.sqrMagnitude < 0.0001f)
            return origin;

        ellipseDir = ellipseDir.normalized * _length;

        return origin + new Vector2(
            ellipseDir.x,
            ellipseDir.y * _yScale
        );
    }

    public bool IsInside(
        Vector2 origin,
        Vector2 dir,
        Vector2 point)
    {
        Vector2 local = point - origin;
        local.y /= _yScale;

        Vector2 forward = new Vector2(dir.x, dir.y / _yScale).normalized;
        Vector2 right = new(-forward.y, forward.x);

        float z = Vector2.Dot(local, forward);
        if (z < 0 || z > _length)
            return false;

        float x = Vector2.Dot(local, right);
        return Mathf.Abs(x) <= _halfWidth;
    }

    // -------------------------------------------------
    // Preview helper
    // -------------------------------------------------

    public AreaLinePreviewData GetPreview(
        Vector2 origin,
        Vector2 end)
    {
        Vector2 dir = end - origin;
        float length = dir.magnitude;

        Vector3 scale = new Vector3(
            _halfWidth * 2f,
            length * _yScale,
            1f
        );

        float angle =
            Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;

        return new AreaLinePreviewData(
            origin,
            end,
            scale,
            angle
        );
    }
}

