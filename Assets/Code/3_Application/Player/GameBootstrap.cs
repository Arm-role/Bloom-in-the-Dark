using System.Collections.Generic;
using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
  private GameApplication _app;

  public void Initialize(DIContainerBase container)
  {
    _app = BuildApplication(container);

    container.Register(_app);

    _app.Start();
  }

  void Update()
  {
    _app.Update();
  }

  void FixedUpdate()
  {
    _app.FixedUpdate();
  }

  private GameApplication BuildApplication(DIContainerBase container)
  {
    var upgradeState = new UpgradeState();
    var gameplayState = new GameplayState();
    var inventoryState = new InventoryState();

    container.Register(upgradeState);
    container.Register(gameplayState);
    container.Register(inventoryState);

    List<IGameState> gameState = new List<IGameState>()
    {
      upgradeState,
      gameplayState,
      inventoryState
    };

    var stateMachine = new GameStateMachine(gameState);

    return new GameApplication(stateMachine);
  }
}