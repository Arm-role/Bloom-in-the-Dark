using TMPro;
using UnityEngine;
using UnityEngine.UI;

// ช่องแสดง item 1 ตัวในหน้า trade — icon + ข้อความจำนวน (มี/ต้องการ)
public class TradeItemSlot : MonoBehaviour
{
  [SerializeField] private Image _icon;
  [SerializeField] private TMP_Text _amountLabel;
  [SerializeField] private Color _enoughColor = Color.white;
  [SerializeField] private Color _notEnoughColor = Color.red;

  public void Bind(int itemId, string amountText, bool enough, IItemIconProvider icons)
  {
    _icon.sprite = icons.GetIcon(itemId);
    _amountLabel.text = amountText;
    _amountLabel.color = enough ? _enoughColor : _notEnoughColor;
  }
}
