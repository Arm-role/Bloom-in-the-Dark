using UnityEngine;
using UnityEngine.UI;

public class OfferingAltarPreview : MonoBehaviour, IOfferingAltarPreview
{
  [SerializeField] private Image _icon;
  [SerializeField] private float _bobAmplitude = 10f;
  [SerializeField] private float _bobSpeed = 2f;

  private RectTransform _rectTransform;
  private Vector2 _basePosition;
  private bool _isVisible;

  private void Awake()
  {
    _rectTransform = _icon.GetComponent<RectTransform>();
    _basePosition = _rectTransform.anchoredPosition;
    Hide();
  }

  private void Update()
  {
    if (!_isVisible) return;

    float bob = Mathf.Sin(Time.time * _bobSpeed) * _bobAmplitude;
    _rectTransform.anchoredPosition = _basePosition + Vector2.up * bob;
  }

  public void Show(Sprite icon)
  {
    if (icon == null) return;
    _icon.sprite = icon;
    _icon.gameObject.SetActive(true);
    _isVisible = true;
  }

  public void Hide()
  {
    _icon.gameObject.SetActive(false);
    _isVisible = false;
  }
}
