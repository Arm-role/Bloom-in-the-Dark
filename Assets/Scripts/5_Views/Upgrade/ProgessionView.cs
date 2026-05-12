using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressionView : MonoBehaviour, IProgressionView
{
  [Header("UI")]
  [SerializeField] private Slider slider;
  [SerializeField] private TextMeshProUGUI levelText;

  [Header("Effect Settings")]
  [SerializeField] private float changeSpeed = 0.5f;

  [Header("Auto Hide")]
  [SerializeField] private bool autoHide = true;
  [SerializeField] private float showDuration = 2f;
  [SerializeField] private float fadeOutDuration = 0.5f;
  [SerializeField] private float fadeInDuration = 0.3f;

  private float _targetValue;

  private bool _isFadingOut;
  private bool _isFadingIn;
  private float _hideTimer;

  private CanvasGroup _canvasGroup;
  private CanvasGroup CG
  {
    get
    {
      if (_canvasGroup != null) return _canvasGroup;
      _canvasGroup = GetComponent<CanvasGroup>();
      if (_canvasGroup == null)
        _canvasGroup = gameObject.AddComponent<CanvasGroup>();
      _canvasGroup.blocksRaycasts = true;
      _canvasGroup.interactable = true;
      return _canvasGroup;
    }
  }

  private void Awake()
  {
    _ = CG;
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
    }

    if (!autoHide) return;

    // ── fade in ───────────────────────────────────────────
    if (_isFadingIn)
    {
      CG.alpha = Mathf.MoveTowards(
          CG.alpha, 1f, Time.deltaTime / fadeInDuration);

      if (CG.alpha >= 1f)
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
      CG.alpha = Mathf.MoveTowards(
          CG.alpha, 0f, Time.deltaTime / fadeOutDuration);

      if (CG.alpha <= 0f)
        _isFadingOut = false;
    }
  }

  // ── view api ─────────────────────────────────────────────
  public void SetProgression(int currentLevel, float currentExp, float maxExp)
  {
    if (maxExp <= 0f) return;

    _targetValue = Mathf.Clamp01(currentExp / maxExp);

    if (levelText != null)
      levelText.text = $"Lv.{currentLevel}";

    if (autoHide)
    {
      CG.alpha = 1f;
      _isFadingIn = false;
      _isFadingOut = false;
      _hideTimer = showDuration;
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
      CG.alpha = 0f;
      _isFadingIn = false;
      _isFadingOut = false;
      _hideTimer = 0f;
    }
  }
}
