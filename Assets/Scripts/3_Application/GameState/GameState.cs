public abstract class GameState
{
  public abstract EGameState State { get; }

  private GameLoop _loop = new GameLoop();

  public void AddSystem(IGameSystem gameSystem)
    => _loop.AddSystem(gameSystem);

  public void ClearSystems() => _loop.Clear();

  public void Enter() => _loop.Enter();
  public void Exit() => _loop.Exit();

  public void Update(float dt) => _loop.Update(dt);
  public void FixedUpdate(float dt) => _loop.FixedUpdate(dt);
}

public class GamePlayState : GameState
{
  public override EGameState State => EGameState.Gameplay;
}
public class InventoryState : GameState
{
  public override EGameState State => EGameState.Inventory;
}
public class UpgradeState : GameState
{
  public override EGameState State => EGameState.Upgrade;
}