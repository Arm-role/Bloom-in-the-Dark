using System.Collections.Generic;
using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
  private GameApplication _app;
   
  public void Initialize(DIContainerBase container)
  {
    _app = BuildApplication(container);

    container.Register(_app);
  }

  public void StartGame()
  {
    _app.Start();
  }

  void Update()
  {
    _app.Update(Time.deltaTime);
  }

  void FixedUpdate()
  {
    _app.FixedUpdate(Time.deltaTime);
  }

  private GameApplication BuildApplication(DIContainerBase container)
  {
    var upgradeState = new UpgradeState();
    var gameplayState = new GamePlayState();
    var inventoryState = new InventoryState();

    List<GameState> gameState = new List<GameState>()
    {
      upgradeState,
      gameplayState,
      inventoryState
    };

    var stateMachine = new GameStateMachine(gameState);

    container.Register(upgradeState);
    container.Register(gameplayState);
    container.Register(inventoryState);

    container.Register(stateMachine);

    return new GameApplication(stateMachine);
  }
}