using System;
using UnityEngine;

public class UpgradeManagerView : MonoBehaviour, IUpgradeManagerView, IUpgradeListener
{
  [SerializeField] private UpgradeMatchService upgradeMatch;
  [SerializeField] private CraftPreviewUI craftPreview;
  public UpgradePopupUI popup;

  private UpgradeManagerPresenter _presenter;

  public event Action OnOpenPopup;
  public event Action OnClosePopup;

  public void Initialze(
    ItemFactory factory,
    GlobalUpgradeDomain upgradeDomain,
    IStatService statService,
    IItemIconProvider iconProvider)
  {
    craftPreview.Initialize(iconProvider);

    _presenter = new UpgradeManagerPresenter(
      factory,
      upgradeDomain,
      upgradeMatch,
      statService,
      popup);

    _presenter.OnSelectUpgrade += SelectUpgrade;
  }

  public void OnOpenUpgradePopup(string upgradeName, int gamekeyId)
  {
    _presenter.OpenUpgradePopup(upgradeName, gamekeyId);
    OnOpenPopup?.Invoke();
  }

  public void SelectUpgrade(int _)
  {
    OnClosePopup?.Invoke();
  }

  public void ShowCraftPreview(UpgradeRequestDefinition request, Action onConfirm)
  {
    craftPreview.Show(request, onConfirm);
  }

  public void HideCraftPreview()
  {
    craftPreview.Hide();
  }
}
