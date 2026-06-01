using UnityEngine;

public class GameApplication
{
  private readonly GameStateMachine _stateMachine;
  private ModalUIStack _modalStack;
  private PassiveModalToken _upgradeModal;

  public GameApplication(GameStateMachine stateMachine)
  {
    _stateMachine = stateMachine;
  }

  public void Initialize(
    IPlayerInput input,
    IUpgradeListener upgradeListener,
    ModalUIStack modalStack)
  {
    _modalStack = modalStack;
    // Upgrade เป็น forced modal — player ต้อง select upgrade ก่อนปิด (HandleDismiss=no-op ใน token)
    // PausesGame=true → stack จัด Time.timeScale=0 ผ่าน OnFirstPausingPush
    _upgradeModal = new PassiveModalToken(modalStack, pausesGame: true);

    upgradeListener.OnOpenPopup += OpenUpgrade;
    upgradeListener.OnClosePopup += CloseUpgrade;
    input.OnInventoryToggle += ToggleInventory;
  }

  public void Start()
  {
    _stateMachine.ChangeState(EGameState.Gameplay);
  }

  // หยุด state machine ตอน scene GamePlay ถูก unload — กัน GameLoop tick destroyed system
  public void ResetForNewScene() => _stateMachine.ResetForNewScene();

  private void OpenUpgrade()
  {
    _upgradeModal.Begin();  // stack push → timeScale=0 + block input
    _stateMachine.ChangeState(EGameState.Upgrade);
  }

  private void CloseUpgrade()
  {
    _upgradeModal.End();    // stack pop → timeScale=1 + unblock
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
