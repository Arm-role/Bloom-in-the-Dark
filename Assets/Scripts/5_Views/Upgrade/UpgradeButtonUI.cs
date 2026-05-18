using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UpgradeButtonUI : MonoBehaviour
{
  [SerializeField] private TMP_Text title;
  [SerializeField] private Button button;

  [SerializeField] private UpgradeModifierUI modifierUIPrefab;
  [SerializeField] private Transform contentRoot;

  private List<UpgradeModifierUI> _upgradeModUI = new();

  public event Action<int> OnClicked;

  private int _cardIndex;

  private void Start()
  {
    button.onClick.AddListener(OnClick);
  }

  public void Setup(int index, IStatPreviewContext context, UpgradeData upgrade)
  {
    foreach (var modUI in _upgradeModUI)
      Destroy(modUI.gameObject);

    _upgradeModUI.Clear();
    _cardIndex = index;

    title.text = upgrade.UpgradeName;

    for (int i = 0; i < upgrade.modifiers.Length; i++)
    {
      var modUI = Instantiate(modifierUIPrefab, contentRoot);
      modUI.Setup(context, upgrade.modifiers[i]);
      _upgradeModUI.Add(modUI);
    }
  }

  public void OnClick()
  {
    OnClicked?.Invoke(_cardIndex);
  }
}
