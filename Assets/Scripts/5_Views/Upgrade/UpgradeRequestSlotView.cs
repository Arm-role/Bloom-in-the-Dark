using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeRequestSlotView : MonoBehaviour
{
  [SerializeField] private TMP_Text amountText;
  [SerializeField] private Image iconImage;

  public void SetIcon(Sprite sprite)
  {
    bool hasIcon = sprite != null;

    iconImage.gameObject.SetActive(hasIcon);
    iconImage.sprite = sprite;
  }

  public void SetAmount(int amount)
  {
    bool show = amount > 1;

    amountText.gameObject.SetActive(show);

    if (show)
      amountText.text = amount.ToString();
  }
}
