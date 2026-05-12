using UnityEngine;

public class OfferingAltarPreview : MonoBehaviour, IOfferingAltarPreview
{
  [Header("Preview Settings")]
  [SerializeField] private SpriteRenderer _iconRenderer;
  [SerializeField] private Vector3 _offset = new Vector3(0f, 1.5f, 0f);
  [SerializeField] private Vector2 _iconSize = new Vector2(1f, 1f);
  [SerializeField] private float _bobAmplitude = 0.1f;
  [SerializeField] private float _bobSpeed = 2f;
  [SerializeField] private float _rotateSpeed = 45f;

  private Vector3 _basePosition;
  private bool _isVisible;

  private void Awake()
  {
    if (_iconRenderer == null)
    {
      var go = new GameObject("AltarPreviewIcon");
      go.transform.SetParent(transform);
      go.transform.localPosition = _offset;
      _iconRenderer = go.AddComponent<SpriteRenderer>();
      _iconRenderer.transform.localScale = new Vector3(_iconSize.x, _iconSize.y, 1f);
    }

    _basePosition = _iconRenderer.transform.localPosition;
    Hide();
  }

  private void Update()
  {
    if (!_isVisible) return;

    // Billboard — หันหน้าหาก Camera ตลอดเวลา
    _iconRenderer.transform.rotation = Camera.main.transform.rotation;

    // Bob ขึ้น-ลง
    float bob = Mathf.Sin(Time.time * _bobSpeed) * _bobAmplitude;
    _iconRenderer.transform.localPosition = _basePosition + Vector3.up * bob;
  }

  public void Show(Sprite icon)
  {
    if (icon == null) return;
    _iconRenderer.sprite = icon;
    _iconRenderer.gameObject.SetActive(true);
    _isVisible = true;
  }

  public void Hide()
  {
    _iconRenderer.gameObject.SetActive(false);
    _isVisible = false;
  }
}