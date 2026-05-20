using System;
using UnityEngine;
using UnityEngine.UI;

public class AltarRecipePreviewUI : MonoBehaviour
{
  [SerializeField] private GameObject _panel;
  [SerializeField] private Image _resultIcon;
  [SerializeField] private Button _confirmButton;

  [Header("Float Animation")]
  [SerializeField] private float _bobAmplitude = 10f;
  [SerializeField] private float _bobSpeed = 2f;

  private IItemIconProvider _iconProvider;
  private Action _onConfirm;
  private Vector2 _iconBasePosition;

  private void Awake()
  {
    _iconBasePosition = _resultIcon.rectTransform.anchoredPosition;
  }

  private void Update()
  {
    // _panel/_resultIcon อาจถูก destroy ตอน scene unload (GameOver) ขณะ view ยัง tick ค้าง
    if (_panel == null || _resultIcon == null || !_panel.activeSelf) return;
    float bob = Mathf.Sin(Time.time * _bobSpeed) * _bobAmplitude;
    _resultIcon.rectTransform.anchoredPosition = _iconBasePosition + Vector2.up * bob;
  }

  public void Initialize(IItemIconProvider iconProvider)
  {
    _iconProvider = iconProvider;
    _confirmButton.onClick.AddListener(OnConfirmClicked);
    Hide();
  }

  public void Show(AltarRecipeDefinition recipe, Action onConfirm)
  {
    _onConfirm = onConfirm;
    _resultIcon.sprite = recipe.GetPreviewIcon(_iconProvider);
    _panel.SetActive(true);
  }

  public void Hide()
  {
    _panel.SetActive(false);
    _onConfirm = null;
  }

  private void OnConfirmClicked()
  {
    _onConfirm?.Invoke();
  }
}
