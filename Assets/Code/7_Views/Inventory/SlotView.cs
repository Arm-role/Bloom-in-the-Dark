using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotView : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private Image SelectedColor;


    public event Action<SlotView> OnSlotClicked;
    public int SlotIndex { get; private set; }

    public void Initialize(int index)
    {
        SlotIndex = index;
    }

    public void Selected(Color color)
    {
        SelectedColor.color = color;
    }
    public void UnSelected(Color color)
    {
        SelectedColor.color = color;
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

    public void OnPointerClick(PointerEventData eventData)
    {
        // ส่ง Event ออกไป บอกว่า "ฉัน (ช่องนี้) ถูกคลิกนะ!"
        OnSlotClicked?.Invoke(this);
    }
}
