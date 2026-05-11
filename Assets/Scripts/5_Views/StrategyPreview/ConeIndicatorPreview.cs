using UnityEngine;

public class ConeIndicatorPreview : MonoBehaviour, IConeIndicatorPreview
{
  [Header("Prefabs")]
  [SerializeField] private GameObject rangePrefab;
  [SerializeField] private GameObject conePrefab;

  [Header("Visual Settings")]
  [SerializeField] private Color rangeColor = new(0f, 1f, 0f, 0.25f);
  [SerializeField] private Color coneColor = new(0f, 0.75f, 1f, 0.4f);

  private GameObject _rangeGO;
  private GameObject _coneGO;

  public void Initialize()
  {
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
}