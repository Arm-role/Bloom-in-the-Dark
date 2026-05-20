using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameBootstrap : MonoBehaviour
{
  private GameApplication _app;

  public void Initialize(DIContainerBase container)
  {
    _app = BuildApplication(container);

    container.Register(_app);
  }

  private void OnEnable() => SceneManager.sceneUnloaded += HandleSceneUnloaded;
  private void OnDisable() => SceneManager.sceneUnloaded -= HandleSceneUnloaded;

  // GameBootstrap เป็น DontDestroyOnLoad → Update ยังรันตอนอยู่ scene อื่น (GameOver/MainMenu)
  // เมื่อ scene GamePlay ถูก unload, system/listener ตายไปกับมัน แต่ GameLoop ยังถือ ref เก่า
  // reset ทันทีเพื่อกัน GameLoop.Update วน tick destroyed MonoBehaviour จน MissingReferenceException
  private void HandleSceneUnloaded(Scene scene)
  {
    _app?.ResetForNewScene();
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