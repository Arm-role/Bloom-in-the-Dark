using UnityEngine;

public class AreaLineIndicatorPreview : MonoBehaviour, IAreaLineIndicatorPreview
{
  [Header("Prefab")]
  [SerializeField] private GameObject linePrefab;

  [Header("Visual Settings")]
  [SerializeField] private Color lineColor = new(0f, 0.75f, 1f, 0.4f);

  [Header("Debug")]
  [SerializeField] private bool _drawGizmo;

  private GameObject _lineGO;

  public void Initialize()
  {
    // สร้างครั้งเดียว — Setup() ถูกเรียกซ้ำทุกครั้งที่เปลี่ยน strategy
    if (_lineGO != null) return;

    _lineGO = CreateIndicator(linePrefab, lineColor);
    Disable();
  }

  public void UpdateView(Vector2 origin, Vector3 scale, float angle)
  {
    _lineGO.transform.position = origin;
    _lineGO.transform.localScale = scale;
    _lineGO.transform.rotation = Quaternion.Euler(CameraConfig.XAngle, 0f, angle);
  }

  public void Enable()
  {
    _lineGO.SetActive(true);
  }

  public void Disable()
  {
    if (_lineGO) _lineGO.SetActive(false);
  }

  private GameObject CreateIndicator(GameObject prefab, Color color)
  {
    var go = Instantiate(prefab, transform);
    var renderer = go.GetComponentInChildren<SpriteRenderer>();
    renderer.color = color;
    return go;
  }

#if UNITY_EDITOR
  // วาดกล่องตาม transform จริงของ indicator — เทียบกับ sprite ใน prefab ว่า pivot/ขนาดตรงไหม
  // เลื่อน +0.5 local Y: beam ยิงจาก origin ไปข้างหน้าเท่านั้น กล่องต้องเริ่มที่ origin ไม่ใช่ center
  private void OnDrawGizmos()
  {
    if (!_drawGizmo) return;
    if (_lineGO == null || !_lineGO.activeSelf) return;

    Gizmos.color = Color.yellow;
    Gizmos.matrix = _lineGO.transform.localToWorldMatrix;
    Gizmos.DrawWireCube(new Vector3(0f, 0.5f, 0f), Vector3.one);
  }
#endif
}
