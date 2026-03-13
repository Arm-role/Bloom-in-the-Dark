using TMPro;
using UnityEngine;

public class UpgradeRequestStoneView : MonoBehaviour
{
  [SerializeField] private TMP_Text amountText;
  [SerializeField] private SpriteRenderer spriteRenderer;

  public void SetIcon(Sprite sprite)
  {
    bool hasIcon = sprite != null;

    spriteRenderer.gameObject.SetActive(hasIcon);
    spriteRenderer.sprite = sprite;
  }

  public void SetAmount(int amount)
  {
    bool show = amount > 1;

    amountText.gameObject.SetActive(show);

    if (show)
      amountText.text = amount.ToString();
  }
}