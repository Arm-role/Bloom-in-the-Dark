using UnityEngine;

public class GameApplication
{
  private readonly GameStateMachine _stateMachine;
  private bool _inventoryOpen;

  public GameApplication(GameStateMachine stateMachine)
  {
    _stateMachine = stateMachine;
  }

  public void Initialize(
    IPlayerInput input,
    IUpgradeListener upgradeListener)
  {
    upgradeListener.OnOpenPopup += OpenUpgrade;
    upgradeListener.OnClosePopup += CloseUpgrade;
    input.OnInventoryToggle += ToggleInventory;
  }

  public void Start()
  {
    _stateMachine.ChangeState(EGameState.Gameplay);
  }

  private void OpenUpgrade()
  {
    Time.timeScale = 0f;
    _stateMachine.ChangeState(EGameState.Upgrade);
  }

  private void CloseUpgrade()
  {
    Time.timeScale = 1f;
    _stateMachine.ChangeState(EGameState.Gameplay);
  }

  private void ToggleInventory()
  {
    if (_stateMachine.CurrentState == EGameState.Upgrade)
      return;

    _inventoryOpen = !_inventoryOpen;

    if (_inventoryOpen)
    {
      _stateMachine.ChangeState(EGameState.Inventory);
    }
    else
    {
      _stateMachine.ChangeState(EGameState.Gameplay);
    }
  }

  public void Update() => _stateMachine.Tick();
  public void FixedUpdate() => _stateMachine.FixedTick();
}