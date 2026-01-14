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
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private Image selectedImage;

    public event Action<int> OnClicked;
    public event Action<int> OnHovered;
    public event Action<int> OnDraggedOver;

    private static bool _isDragging;
    public int SlotIndex { get; private set; }

    public void Initialize(int index)
    {
        SlotIndex = index;
        Unselect(Color.clear);
    }

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

        if (!_isDragging)
            return;

        OnDraggedOver?.Invoke(SlotIndex);
    }
    public void UpdateView(Sprite icon, int amount)
    {
        bool hasItem = icon != null && amount > 0;

        iconImage.gameObject.SetActive(hasItem);
        amountText.gameObject.SetActive(hasItem && amount > 1);

        if (hasItem)
        {
            iconImage.sprite = icon;
            amountText.text = amount.ToString();
        }
    }

    public void Clear()
    {
        iconImage.gameObject.SetActive(false);
        amountText.gameObject.SetActive(false);
    }

    public void Select(Color color)
    {
        selectedImage.color = color;
    }

    public void Unselect(Color color)
    {
        selectedImage.color = color;
    }


}
