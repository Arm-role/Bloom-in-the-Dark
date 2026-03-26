using System;
using System.Collections.Generic;

public class UpgradeManagerPresenter
{
  private readonly ItemFactory _itemFactory;
  private readonly GlobalUpgradeDomain _domain;
  private readonly UpgradeMatchService _manager;
  private readonly IStatService _statService;
  private readonly UpgradePopupUI _popup;

  private List<UpgradeData> _upgradeData;

  public event Action<int> OnSelectUpgrade;

  public UpgradeManagerPresenter(
    ItemFactory itemFactory,
    GlobalUpgradeDomain domain,
    UpgradeMatchService manager,
    IStatService statService,
    UpgradePopupUI popup)
  {
    _itemFactory = itemFactory;
    _domain = domain;
    _manager = manager;
    _statService = statService;
    _popup = popup;

    _popup.OnSelecetUpgrade += SelectUpgrade;
  }

  public void OpenUpgradePopup(int gamekeyId)
  {
    bool haveUpgrade = _manager.TryGetUpgradesData(gamekeyId, 3, out _upgradeData);

    if (!haveUpgrade) return;

    IStatPreviewContext context;

    var item = _itemFactory.Create(gamekeyId);

    if (item != null)
    {
      context = new ItemStatPreviewContext(item.Stats);
      _popup.Show(context, _upgradeData);

      return;
    }

    context = new RunStatPreviewContext(_statService);
    _popup.Show(context, _upgradeData);

  }

  public void SelectUpgrade(int upgradeIndex)
  {
    _domain.AddUpgrade(_upgradeData[upgradeIndex]);
    _popup.Hide();
    OnSelectUpgrade?.Invoke(upgradeIndex);
  }
}