using UnityEngine;

public class ConeShape
{
  private float _range;
  private float _halfAngleDeg;
  private float _yScale;

  public float Range => _range;
  public float YScale => _yScale;

  public void Setup(float xAngle, float range, float angleDeg)
  {
    _range = range;
    _halfAngleDeg = angleDeg * 0.5f;
    _yScale = Mathf.Cos(xAngle * Mathf.Deg2Rad);
  }

  // ---------------------------------------------------
  // Geometry
  // ---------------------------------------------------

  public bool IsInside(Vector2 origin, Vector2 forward, Vector2 point)
  {
    Vector2 local = point - origin;

    Vector2 localE = new Vector2(local.x, local.y / _yScale);
    Vector2 forwardE = new Vector2(forward.x, forward.y / _yScale).normalized;

    if (localE.magnitude > _range)
      return false;

    if (localE.sqrMagnitude < 0.0001f)
      return true;

    float angle = Vector2.Angle(forwardE, localE);
    return angle <= _halfAngleDeg;
  }

  public Vector2 GetWorldDirection(Vector2 origin, Vector2 pointer)
  {
    Vector2 dir = pointer - origin;

    if (dir.sqrMagnitude < 0.0001f)
      return Vector2.right;

    return dir.normalized;
  }

  // ---------------------------------------------------
  // Preview helper
  // ---------------------------------------------------

  public ConePreviewData GetPreview(Vector2 origin, Vector2 pointer)
  {
    Vector2 dir = GetWorldDirection(origin, pointer);

    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;

    // Range ellipse (ไม่เปลี่ยน)
    Vector3 rangeScale = new Vector3(
        _range * 2f,
        _range * 2f * _yScale,
        1f);

    float halfAngleRad = _halfAngleDeg * Mathf.Deg2Rad;
    float coneUniformScale = _range * 2f * Mathf.Tan(halfAngleRad);

    Vector3 coneScale = new Vector3(
        coneUniformScale,
        coneUniformScale,
        1f);

    return new ConePreviewData(origin, dir, rangeScale, coneScale, angle);
  }
}