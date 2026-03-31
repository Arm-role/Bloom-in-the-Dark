using System;
using UnityEngine;

public class UpgradeManagerView : MonoBehaviour, IUpgradeManagerView, IUpgradeListener
{
  [SerializeField] private UpgradeMatchService upgradeMatch;
  public UpgradePopupUI popup;

  private UpgradeManagerPresenter _presenter;

  public event Action OnOpenPopup;
  public event Action OnClosePopup;

  public void Initialze(
    ItemFactory factory,
    GlobalUpgradeDomain upgradeDomain,
    IStatService statService)
  {
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
}
