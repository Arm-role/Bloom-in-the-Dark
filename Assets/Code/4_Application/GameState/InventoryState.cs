public class InventoryState : IGameState
{
  public EGameState Type => EGameState.Inventory;

  private InventoryScreenController _inventoryScreen;
  private PlayerActionLock _actionLock;
  private IPlayerController _playerController;

  public void Initialize(
    InventoryScreenController inventoryScreen,
    PlayerActionLock actionLock,
    IPlayerController playerController)
  {
    _inventoryScreen = inventoryScreen;
    _actionLock = actionLock;
    _playerController = playerController;
  }
  public void Enter() => _inventoryScreen.Open();
  public void Exit() => _inventoryScreen.Close();

  public void Tick()
  {
    _actionLock.Update();
    _playerController.ManualUpdate();
  }
  public void FixedTick()
  {
    _playerController?.ManualFixedUpdate();
  }
}