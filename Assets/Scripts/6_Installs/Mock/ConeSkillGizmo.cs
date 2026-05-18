#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class ConeSkillGizmo : MonoBehaviour
{
  [Header("Shape")]
  public float xAngle = 55f;
  public float range = 6f;
  public float angleDeg = 70f;

  [Header("Preview")]
  public Vector2 direction = Vector2.up;
  public Color ellipseColor = new Color(0f, 1f, 1f, 0.3f);
  public Color coneColor = new Color(1f, 0.5f, 0f, 0.5f);
  public Color hitColor = Color.red;

  private ConeShape _shape = new();

  void OnValidate()
  {
    _shape.Setup(xAngle, range, angleDeg);
  }

void OnDrawGizmos()
{
    if (_shape == null) { _shape = new ConeShape(); _shape.Setup(xAngle, range, angleDeg); }

    Vector2 origin = transform.position;
    Vector2 dir = GetMouseDirection(origin);  // ← เปลี่ยนตรงนี้

    DrawEllipse(origin);
    DrawCone(origin, dir);
    DrawDirectionArrow(origin, dir);
    DrawAngleArc(origin, dir);
}

Vector2 GetMouseDirection(Vector2 origin)
{
#if UNITY_EDITOR
    // ดึง mouse position ใน world space จาก SceneView
    SceneView sv = SceneView.currentDrawingSceneView;
    if (sv == null) return direction.normalized;

    // แปลง mouse pos (screen) → world pos
    Vector2 mouseScreen = Event.current?.mousePosition ?? Vector2.zero;
    // SceneView ใช้ GUI coordinates (Y กลับ)
    mouseScreen.y = sv.camera.pixelHeight - mouseScreen.y;
    Ray ray = sv.camera.ScreenPointToRay(mouseScreen);
    Vector2 mouseWorld = ray.origin;

    Vector2 d = mouseWorld - origin;
    return d.sqrMagnitude > 0.001f ? d.normalized : direction.normalized;
#else
    return direction.normalized;
#endif
}

  // วาด ellipse range
  void DrawEllipse(Vector2 origin)
  {
    int segments = 64;
    Gizmos.color = ellipseColor;

    Vector3 prev = Vector3.zero;
    for (int i = 0; i <= segments; i++)
    {
      float t = i / (float)segments * Mathf.PI * 2f;
      float ex = Mathf.Cos(t) * range;
      float ey = Mathf.Sin(t) * range * _shape.YScale;
      Vector3 p = new Vector3(origin.x + ex, origin.y + ey, 0f);

      if (i > 0) Gizmos.DrawLine(prev, p);
      prev = p;
    }
  }

  // วาด cone สองเส้นขอบ + arc ที่ฐาน
  void DrawCone(Vector2 origin, Vector2 dir)
  {
    float halfRad = angleDeg * 0.5f * Mathf.Deg2Rad;
    float yScale = _shape.YScale;

    Gizmos.color = coneColor;

    int steps = 32;
    Vector3 prevLeft = Vector3.zero;
    Vector3 prevRight = Vector3.zero;

    for (int i = 0; i <= steps; i++)
    {
      float t = i / (float)steps; // 0 = origin, 1 = rim

      // หมุน dir ± halfAngle
      Vector2 leftDir = Rotate(dir, halfRad);
      Vector2 rightDir = Rotate(dir, -halfRad);

      // scale ตาม ellipse ณ ตำแหน่ง t ตามแนว dir
      float eLenLeft = GetEllipseRadius(leftDir, yScale) * t;
      float eLenRight = GetEllipseRadius(rightDir, yScale) * t;

      Vector3 pLeft = (Vector3)(origin + leftDir * eLenLeft) + Vector3.zero;
      Vector3 pRight = (Vector3)(origin + rightDir * eLenRight) + Vector3.zero;

      if (i > 0)
      {
        Gizmos.DrawLine(prevLeft, pLeft);
        Gizmos.DrawLine(prevRight, pRight);
      }
      prevLeft = pLeft;
      prevRight = pRight;
    }

    // เส้นจาก origin ถึงขอบ
    Gizmos.DrawLine(origin, prevLeft);
    Gizmos.DrawLine(origin, prevRight);

    // arc ที่ฐาน cone
    DrawConeArc(origin, dir, yScale);
  }

  void DrawConeArc(Vector2 origin, Vector2 dir, float yScale)
  {
    float halfRad = angleDeg * 0.5f * Mathf.Deg2Rad;
    int steps = 20;

    Vector3 prev = Vector3.zero;
    for (int i = 0; i <= steps; i++)
    {
      float t = -halfRad + (halfRad * 2f) * (i / (float)steps);
      Vector2 d = Rotate(dir, t);
      float r = GetEllipseRadius(d, yScale);
      Vector3 point = new Vector3(origin.x + d.x * r, origin.y + d.y * r, 0f);

      if (i > 0) Gizmos.DrawLine(prev, point);
      prev = point;
    }
  }

  // ลูกศรทิศทาง
  void DrawDirectionArrow(Vector2 origin, Vector2 dir)
  {
    Gizmos.color = Color.white;
    float r = GetEllipseRadius(dir, _shape.YScale);
    Vector3 tip = new Vector3(origin.x + dir.x * r, origin.y + dir.y * r, 0f);

    Gizmos.DrawLine(origin, tip);

    // หัวลูกศร
    Vector2 left = Rotate(dir, 20f * Mathf.Deg2Rad) * 0.3f;
    Vector2 right = Rotate(dir, -20f * Mathf.Deg2Rad) * 0.3f;
    Gizmos.DrawLine(tip, tip - (Vector3)left);
    Gizmos.DrawLine(tip, tip - (Vector3)right);
  }

  // arc แสดง angle
  void DrawAngleArc(Vector2 origin, Vector2 dir)
  {
    float halfRad = angleDeg * 0.5f * Mathf.Deg2Rad;
    float arcR = range * 0.25f;

    Handles.color = new Color(1f, 1f, 0f, 0.8f);
    Handles.DrawWireArc(
        origin,
        Vector3.forward,
        Rotate(dir, -halfRad),
        angleDeg,
        arcR
    );

    // label
    Vector2 labelPos = origin + dir * arcR * 1.4f;
    Handles.Label(labelPos, $"{angleDeg}°");
  }

  // ------- Helpers -------

  Vector2 Rotate(Vector2 v, float rad)
  {
    float cos = Mathf.Cos(rad);
    float sin = Mathf.Sin(rad);
    return new Vector2(
        v.x * cos - v.y * sin,
        v.x * sin + v.y * cos
    );
  }

  float GetEllipseRadius(Vector2 dir, float yScale)
  {
    float dx = dir.x;
    float dy = dir.y;
    float denom = Mathf.Sqrt(dx * dx + (dy * dy) / (yScale * yScale));
    return denom < 0.0001f ? 0f : range / denom;
  }
}
#endif