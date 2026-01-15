public class GameplayState : IGameState
{
    public EGameState Type => EGameState.Gameplay;

    private IPlayerController _playerController;
    private IDragDropController _dragDropController;

    public void Initialize(
        IPlayerController playerController,
        IDragDropController dragDropController)
    {
        _playerController = playerController;
        _dragDropController = dragDropController;
    }
    public void Tick()
    {
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
