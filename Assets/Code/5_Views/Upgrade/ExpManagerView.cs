using UnityEngine;

public class ExpManagerView : MonoBehaviour
{
  [SerializeField] private GlobalKey baseKey;
  [SerializeField] private HealthBar _expBar;

  private PlayerProgression _progression;
  private ExpBarPresenter _presenter;
  private IUpgradeManagerView _upgradeManager;

  public void Initialze(
    PlayerProgression playerProgression, 
    IUpgradeManagerView upgradeManager)
  {
    _upgradeManager = upgradeManager;

    _progression = playerProgression;
    _presenter = new ExpBarPresenter(_progression, _expBar);

    _progression.OnLevelUp += HandleLevelUp;
  }

  public void Update()
  {
    if (Input.GetKeyDown(KeyCode.U))
    {
      _progression.AddExp(30);
    }
  }

  private void HandleLevelUp(int level)
  {
    _upgradeManager.OnOpenPopup(baseKey.RuntimeTag.Hash);
  }

  public void AddExp(float amount)
  {
    _progression.AddExp(amount);
  }

  private void OnDestroy()
  {
    _presenter.Dispose();
    _progression.OnLevelUp -= HandleLevelUp;
  }
}