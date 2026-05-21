using UnityEngine;

public class ConeIndicatorPreview : MonoBehaviour, IConeIndicatorPreview
{
  [Header("Prefabs")]
  [SerializeField] private GameObject rangePrefab;
  [SerializeField] private GameObject conePrefab;

  [Header("Visual Settings")]
  [SerializeField] private Color rangeColor = new(0f, 1f, 0f, 0.25f);
  [SerializeField] private Color coneColor = new(0f, 0.75f, 1f, 0.4f);

  [Header("Debug")]
  [SerializeField] private bool _drawGizmo;

  private GameObject _rangeGO;
  private GameObject _coneGO;

  public void Initialize()
  {
    // สร้างครั้งเดียว — Setup() ถูกเรียกซ้ำทุกครั้งที่เปลี่ยน strategy
    // ถ้าไม่ guard GameObject เก่าจะค้างสะสมใต้ transform เรื่อยๆ
    if (_rangeGO != null) return;

    _rangeGO = CreateIndicator(rangePrefab, rangeColor);
    _coneGO = CreateIndicator(conePrefab, coneColor);
    Disable();
  }

  public void UpdateView(
      Vector2 origin,
      Vector2 direction,
      Vector3 rangeScale,
      Vector3 coneScale,
      float angle)
  {
    // Range ellipse
    _rangeGO.transform.position = origin;
    _rangeGO.transform.localScale = rangeScale;

    _coneGO.transform.position = origin;
    _coneGO.transform.localScale = coneScale;
    _coneGO.transform.rotation = Quaternion.Euler(CameraConfig.XAngle, 0f, angle);

#if UNITY_EDITOR
    CacheGizmoGeometry(origin, direction, rangeScale, coneScale);
#endif
  }

  public void Enable()
  {
    _rangeGO.SetActive(true);
    _coneGO.SetActive(true);
  }

  public void Disable()
  {
    if (_rangeGO) _rangeGO.SetActive(false);
    if (_coneGO) _coneGO.SetActive(false);
  }

  private GameObject CreateIndicator(GameObject prefab, Color color)
  {
    var go = Instantiate(prefab, transform);
    var renderer = go.GetComponentInChildren<SpriteRenderer>();
    renderer.color = color;
    return go;
  }

#if UNITY_EDITOR
  private Vector2 _gizmoOrigin;
  private Vector2 _gizmoForwardE;
  private float _gizmoRange;
  private float _gizmoHalfAngleRad;
  private float _gizmoYScale = 1f;

  // reconstruct geometry จริงของ cone จาก scale — scale มาจาก ConeShape ที่ Setup ด้วยค่า range/angle ของ SO
  // rangeScale.x = range*2 ; coneScale.x/rangeScale.x = tan(halfAngle) ; rangeScale.y/rangeScale.x = yScale
  private void CacheGizmoGeometry(
    Vector2 origin,
    Vector2 direction,
    Vector3 rangeScale,
    Vector3 coneScale)
  {
    _gizmoOrigin = origin;

    if (Mathf.Approximately(rangeScale.x, 0f))
    {
      _gizmoRange = 0f;
      return;
    }

    _gizmoRange = rangeScale.x * 0.5f;
    _gizmoYScale = rangeScale.y / rangeScale.x;
    _gizmoHalfAngleRad = Mathf.Atan(coneScale.x / rangeScale.x);
    _gizmoForwardE = new Vector2(direction.x, direction.y / _gizmoYScale).normalized;
  }

  // วาด wedge สามเหลี่ยมจริงของ cone (apex ที่ origin) ตาม range/angle ของ SO
  private void OnDrawGizmos()
  {
    if (!_drawGizmo) return;

    // วงระยะ — center ถูกต้อง (วงรอบตัว player)
    if (_rangeGO != null && _rangeGO.activeSelf)
    {
      Gizmos.color = Color.yellow;
      Gizmos.matrix = _rangeGO.transform.localToWorldMatrix;
      Gizmos.DrawWireSphere(Vector3.zero, 0.5f);
    }

    if (_coneGO != null && _coneGO.activeSelf && _gizmoRange > 0f)
    {
      Gizmos.color = Color.cyan;
      Gizmos.matrix = Matrix4x4.identity;
      DrawConeWedge();
    }
  }

  private void DrawConeWedge()
  {
    const int segments = 20;

    float baseAngle = Mathf.Atan2(_gizmoForwardE.y, _gizmoForwardE.x);
    Vector3 apex = _gizmoOrigin;
    Vector3 prev = apex;

    for (int i = 0; i <= segments; i++)
    {
      float t = Mathf.Lerp(-_gizmoHalfAngleRad, _gizmoHalfAngleRad, i / (float)segments);
      float a = baseAngle + t;

      // จุดบนส่วนโค้ง: คำนวณใน ellipse space แล้ว map กลับ world ด้วย yScale (วงรีตาม isometric)
      Vector3 edge = apex + new Vector3(
        Mathf.Cos(a) * _gizmoRange,
        Mathf.Sin(a) * _gizmoRange * _gizmoYScale,
        0f);

      if (i == 0)
        Gizmos.DrawLine(apex, edge);   // ขอบด้านหนึ่งจาก apex
      else
        Gizmos.DrawLine(prev, edge);   // ส่วนโค้งปลาย cone

      prev = edge;
    }

    Gizmos.DrawLine(prev, apex);       // ขอบอีกด้านกลับเข้า apex
  }
#endif
}