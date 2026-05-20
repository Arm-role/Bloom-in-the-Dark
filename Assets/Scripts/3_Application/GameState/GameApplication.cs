using UnityEngine;

public class GameApplication
{
  private readonly GameStateMachine _stateMachine;

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

    // block ตอน respawn
    if (GameSession.IsPlayerRespawning)
      return;

    if (_stateMachine.CurrentState == EGameState.Inventory)
    {
      _stateMachine.ChangeState(EGameState.Gameplay);
    }
    else if (_stateMachine.CurrentState == EGameState.Gameplay)
    {
      _stateMachine.ChangeState(EGameState.Inventory);
    }
  }

  public void Update(float dt) => _stateMachine.Update(dt);
  public void FixedUpdate(float dt) => _stateMachine.FixedUpdate(dt);
}