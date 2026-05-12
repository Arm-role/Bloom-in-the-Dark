using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressionView : MonoBehaviour, IProgressionView
{
  [Header("UI")]
  [SerializeField] private Slider slider;
  [SerializeField] private TextMeshProUGUI levelText;
  [SerializeField] private CanvasGroup _canvasGroup;

  [Header("Effect Settings")]
  [SerializeField] private float changeSpeed = 0.5f;

  [Header("Auto Hide")]
  [SerializeField] private bool autoHide = true;
  [SerializeField] private float showDuration = 2f;
  [SerializeField] private float fadeOutDuration = 0.5f;
  [SerializeField] private float fadeInDuration = 0.3f;

  public event System.Action OnFilled;

  private float _targetValue;
  private bool _notifyOnFilled;

  private bool _isFadingOut;
  private bool _isFadingIn;
  private float _hideTimer;


  private void Awake()
  {
    _ = _canvasGroup;
    slider.interactable = false;
    _targetValue = slider.value;
  }

  private void Update()
  {
    // ── smooth slider ─────────────────────────────────────
    if (!Mathf.Approximately(slider.value, _targetValue))
    {
      slider.value = Mathf.MoveTowards(
          slider.value, _targetValue, changeSpeed * Time.deltaTime);

      if (_notifyOnFilled && Mathf.Approximately(slider.value, _targetValue))
      {
        _notifyOnFilled = false;
        _hideTimer = showDuration;  // start normal countdown after fill completes
        OnFilled?.Invoke();
      }
    }

    // Pause the auto-hide countdown while waiting for the fill animation to fire OnFilled.
    if (_notifyOnFilled) return;

    if (!autoHide) return;

    // ── fade in ───────────────────────────────────────────
    if (_isFadingIn)
    {
      _canvasGroup.alpha = Mathf.MoveTowards(
          _canvasGroup.alpha, 1f, Time.deltaTime / fadeInDuration);

      if (_canvasGroup.alpha >= 1f)
      {
        _isFadingIn = false;
        _hideTimer = showDuration;
      }
      return;
    }

    // ── countdown → fade out ──────────────────────────────
    if (_hideTimer > 0f)
    {
      _hideTimer -= Time.deltaTime;
      if (_hideTimer <= 0f)
        _isFadingOut = true;
    }

    if (_isFadingOut)
    {
      _canvasGroup.alpha = Mathf.MoveTowards(
          _canvasGroup.alpha, 0f, Time.deltaTime / fadeOutDuration);

      if (_canvasGroup.alpha <= 0f)
        _isFadingOut = false;
    }
  }

  // ── view api ─────────────────────────────────────────────
  public void SetProgression(int currentLevel, float currentExp, float maxExp)
  {
    Debug.Log($"SetProgression called with: Level={currentLevel}, CurrentExp={currentExp}, MaxExp={maxExp}");
    if (maxExp <= 0f) return;

    _targetValue = Mathf.Clamp01(currentExp / maxExp);
    _notifyOnFilled = Mathf.Approximately(_targetValue, 1f);

    if (levelText != null)
      levelText.text = $"Lv.{currentLevel}";

    if (autoHide)
    {
      _canvasGroup.alpha = 1f;
      _isFadingIn = false;
      _isFadingOut = false;
      // Keep the bar visible until OnFilled fires; after that the normal
      // showDuration countdown takes over.
      _hideTimer = _notifyOnFilled ? float.MaxValue : showDuration;
    }
  }

  public void SetProgressionImmediate(int currentLevel, float currentExp, float maxExp)
  {
    if (maxExp <= 0f) return;

    float normalized = Mathf.Clamp01(currentExp / maxExp);
    _targetValue = normalized;
    slider.value = normalized;

    if (levelText != null)
      levelText.text = $"Lv.{currentLevel}";

    if (autoHide)
    {
      _canvasGroup.alpha = 0f;
      _isFadingIn = false;
      _isFadingOut = false;
      _hideTimer = 0f;
    }
  }
}
