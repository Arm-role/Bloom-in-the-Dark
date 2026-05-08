using UnityEngine;

public class ExpManagerView : MonoBehaviour
{
  [SerializeField] private GlobalKey baseKey;
  [SerializeField] private HealthBar _expBar;

  private PlayerProgression _progression;
  private ExpBarPresenter _presenter;
  private IUpgradeManagerView _upgradeManager;
  private IUpgradeListener _upgradeListener;

  private int _pendingLevelUps;
  private bool _isShowingUpgrade;

  public void Initialze(
    PlayerProgression playerProgression,
    IUpgradeManagerView upgradeManager,
    IUpgradeListener upgradeListener)
  {
    _upgradeManager = upgradeManager;
    _upgradeListener = upgradeListener;

    _progression = playerProgression;
    _presenter = new ExpBarPresenter(_progression, _expBar);

    _progression.OnLevelUp += HandleLevelUp;
    _upgradeListener.OnClosePopup += HandleUpgradeClosed;
  }

  private void HandleLevelUp(int level)
  {
    _pendingLevelUps++;

    TryShowNextUpgrade();
  }

  public void AddExp(float amount)
  {
    _progression.AddExp(amount);
  }
  private void HandleUpgradeClosed()
  {
    _isShowingUpgrade = false;
    TryShowNextUpgrade();
  }

  private void TryShowNextUpgrade()
  {
    if (_isShowingUpgrade) return;
    if (_pendingLevelUps <= 0) return;

    _pendingLevelUps--;
    _isShowingUpgrade = true;

    _upgradeManager.OnOpenUpgradePopup($"Level UP {_progression.Level - _pendingLevelUps}", baseKey.RuntimeTag.Hash);
  }
  private void OnDestroy()
  {
    _presenter.Dispose();
    _progression.OnLevelUp -= HandleLevelUp;
  }
}