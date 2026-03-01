using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotView : MonoBehaviour,
    IPointerClickHandler,
    IPointerDownHandler,
    IPointerUpHandler,
    IPointerEnterHandler
{
  [Header("UI")]
  [SerializeField] private Image iconImage;
  [SerializeField] private TextMeshProUGUI amountText;
  [SerializeField] private Image selectedImage;

  [Header("Cooldown UI")]
  [SerializeField] private GameObject cooldownRoot;
  [SerializeField] private Image cooldownFill;
  [SerializeField] private TextMeshProUGUI cooldownText;

  public event Action<int> OnClicked;
  public event Action<int> OnHovered;
  public event Action<int> OnDraggedOver;

  private static bool _isDragging;
  public int SlotIndex { get; private set; }

  // =============================
  // Initialization
  // =============================

  public void Initialize(int index)
  {
    SlotIndex = index;
    Clear();
    SetHighlight(Color.clear);
    SetCooldown(0f);
  }

  // =============================
  // Pointer Events (Forward Only)
  // =============================

  public void OnPointerClick(PointerEventData eventData)
  {
    OnClicked?.Invoke(SlotIndex);
  }

  public void OnPointerDown(PointerEventData eventData)
  {
    if (eventData.button == PointerEventData.InputButton.Left)
      _isDragging = true;

    OnDraggedOver?.Invoke(SlotIndex);
  }

  public void OnPointerUp(PointerEventData eventData)
  {
    if (eventData.button == PointerEventData.InputButton.Left)
      _isDragging = false;
  }

  public void OnPointerEnter(PointerEventData eventData)
  {
    OnHovered?.Invoke(SlotIndex);

    if (_isDragging)
      OnDraggedOver?.Invoke(SlotIndex);
  }

  // =============================
  // Rendering API (Called by View)
  // =============================

  public void SetIcon(Sprite sprite)
  {
    bool hasIcon = sprite != null;

    Debug.Log($"hasIcon {hasIcon}");
    iconImage.gameObject.SetActive(hasIcon);
    iconImage.sprite = sprite;
  }

  public void SetAmount(int amount)
  {
    bool show = amount > 1;
    Debug.Log($"hasAmount {show}");

    amountText.gameObject.SetActive(show);

    if (show)
      amountText.text = amount.ToString();
  }

  public void Clear()
  {
    iconImage.gameObject.SetActive(false);
    amountText.gameObject.SetActive(false);
    SetCooldown(0f);
  }

  public void SetHighlight(Color color)
  {
    selectedImage.color = color;
  }

  public void SetCooldown(float normalized)
  {
    if (normalized <= 0f)
    {
      cooldownRoot.SetActive(false);
      return;
    }

    cooldownRoot.SetActive(true);
    cooldownFill.fillAmount = normalized;

    float remaining = 1f - normalized;
    cooldownText.text = Mathf.Ceil(remaining * 10f).ToString();
  }
}
