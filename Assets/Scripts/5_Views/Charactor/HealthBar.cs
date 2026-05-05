using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour, IBarView, IPointerClickHandler
{
  [SerializeField] private string barName;

  [Header("UI")]
  [SerializeField] private Image fillTop;
  [SerializeField] private Image fillBottom;
  [SerializeField] private TextMeshProUGUI amountText;

  [Header("Effect Settings")]
  [SerializeField] private float damageDelaySpeed = 0.5f;
  [SerializeField] private float healDelaySpeed = 0.8f;

  [Header("Auto Hide")]
  [SerializeField] private bool autoHide = true;
  [SerializeField] private float showDuration = 2f;
  [SerializeField] private float fadeOutDuration = 0.5f;
  [SerializeField] private float fadeInDuration = 0.3f;

  public string Name => barName;

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

  private void Awake() => _ = CG;

  private void Update()
  {
    // ── delayed fill bar ──────────────────────────────────
    float target = fillTop.fillAmount;
    float current = fillBottom.fillAmount;

    if (!Mathf.Approximately(current, target))
    {
      float speed = current > target ? damageDelaySpeed : healDelaySpeed;
      fillBottom.fillAmount = Mathf.MoveTowards(
          current, target, speed * Time.deltaTime);
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
        _hideTimer = showDuration; // เริ่มนับหลัง fade in เสร็จ
      }
      return;
    }

    // ── นับถอยหลัง → fade out ─────────────────────────────
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

  // ── pointer click ─────────────────────────────────────────
  public void OnPointerClick(PointerEventData eventData)
  {
    // คลิกซ้ำตอนแสดงอยู่ → reset timer
    if (CG.alpha >= 1f)
    {
      _hideTimer = showDuration;
      _isFadingOut = false;
      return;
    }

    // คลิกตอนซ่อนอยู่ → fade in
    _isFadingIn = true;
    _isFadingOut = false;
    _hideTimer = 0f;
  }

  // ── view api ─────────────────────────────────────────────
  public void SetHealth(float current, float max)
  {
    if (max <= 0f) return;

    fillTop.fillAmount = Mathf.Clamp01(current / max);

    if (amountText != null)
      amountText.text =
          $"{Mathf.CeilToInt(current)}/{Mathf.CeilToInt(max)}";

    if (autoHide)
    {
      // damage/heal → pop ขึ้นทันที
      CG.alpha = 1f;
      _isFadingIn = false;
      _isFadingOut = false;
      _hideTimer = showDuration;
    }
  }

  public void SetHealthImmediate(float current, float max)
  {
    if (max <= 0f) return;

    float normalized = Mathf.Clamp01(current / max);
    fillTop.fillAmount = normalized;
    fillBottom.fillAmount = normalized;

    if (amountText != null)
      amountText.text =
          $"{Mathf.CeilToInt(current)}/{Mathf.CeilToInt(max)}";

    if (autoHide)
    {
      CG.alpha = 0f;
      _isFadingIn = false;
      _isFadingOut = false;
      _hideTimer = 0f;
    }
  }
}