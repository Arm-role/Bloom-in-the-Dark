using System;
using UnityEngine;

public class UpgradeManagerView : MonoBehaviour, IUpgradeManagerView, IUpgradeListener
{
  [SerializeField] private UpgradeMatchService upgradeMatch;
  public UpgradePopupUI popup;

  private UpgradeManagerPresenter _presenter;

  public event Action OnOpenUpgradePopup;
  public event Action<int> OnSelectUpgrade;

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

    _presenter.OnOpenUpgradePopup += () => { OnOpenUpgradePopup?.Invoke(); };
    _presenter.OnSelectUpgrade += (va) => { OnSelectUpgrade?.Invoke(va); };
  }

  public void OnOpenPopup(int gamekeyId)
   => _presenter.OpenUpgradePopup(gamekeyId);
}
