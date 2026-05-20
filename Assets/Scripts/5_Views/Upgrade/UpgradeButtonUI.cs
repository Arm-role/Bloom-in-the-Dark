using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButtonUI : MonoBehaviour
{
  [SerializeField] private Image cardImage;
  [SerializeField] private TMP_Text title;
  [SerializeField] private TMP_Text description;
  [SerializeField] private Button button;

  public event Action<int> OnClicked;

  private int _cardIndex;

  private void Start()
  {
    button.onClick.AddListener(OnClick);
  }

  public void Setup(int index, IStatPreviewContext context, UpgradeData upgrade)
  {
    _cardIndex = index;

    cardImage.sprite = upgrade.CardSprite;
    title.text = upgrade.UpgradeName;
    description.text = BuildDescription(context, upgrade);
  }

  // description = format string — {0}{1}=modifier[0] before/after, {2}{3}=modifier[1] ...
  private static string BuildDescription(IStatPreviewContext context, UpgradeData upgrade)
  {
    var mods = upgrade.modifiers;
    if (mods == null || mods.Length == 0)
      return upgrade.Description;

    var args = new object[mods.Length * 2];
    for (int i = 0; i < mods.Length; i++)
    {
      args[i * 2] = context.GetBefore(mods[i].StatKey);
      args[i * 2 + 1] = context.GetAfter(mods[i]);
    }
    return string.Format(upgrade.Description, args);
  }

  public void OnClick()
  {
    OnClicked?.Invoke(_cardIndex);
  }
}
