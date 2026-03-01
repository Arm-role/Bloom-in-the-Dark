public class GameplayState : IGameState
{
  public EGameState Type => EGameState.Gameplay;

  private PlayerActionLock _actionLock;
  private IPlayerController _playerController;
  private IDragDropController _dragDropController;

  public void Initialize(
    PlayerActionLock actionLock,
    IPlayerController playerController,
    IDragDropController dragDropController)
  {
    _actionLock = actionLock;
    _playerController = playerController;
    _dragDropController = dragDropController;
  }
  public void Tick()
  {
    _actionLock.Update();
    _playerController.ManualUpdate();
    _dragDropController.ManualUpdate();
  }
  public void FixedTick()
  {
    _playerController?.ManualFixedUpdate();
  }

  public void Enter() { }
  public void Exit() { }
}
