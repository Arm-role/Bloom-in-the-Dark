public class SceneBindingInstaller
{
  public void Install(DIContainerBase container, GameSceneInstaller scene)
  {
    var gameApplication = container.Get<GameApplication>();

    var state = container.Get<PlayerState>();
    var data = container.Get<PlayerData>();

    var hotbarState = container.Get<HotbarState>();
    var inventory = container.Get<PlayerInventory>();
    
    var health = container.Get<HealthResource>();
    var energy = container.Get<PlayerEnergy>();
    var interactor = container.Get<PlayerInteractor>();
    

    var cellResoulver = container.Get<DefaultCellActionResolver>();
    var costResolver = container.Get<InteractionCostResolver>();
    var excutor = container.Get<WorldInteractionExecutor>();

    var handleService = container.Get<InteractionHandleService>();
    var playerState = container.Get<PlayerState>();
    var particalService = container.Get<ParticalService>();

    var spawnerHandle = container.Get<SpawnerHandle>();

    gameApplication.Initialize(scene.InputRender);

    // =======================
    // Player
    // =======================

    scene.PlayerController.Initialize(
      scene.InputRender,
      data,
      state,
      health,
      energy,
      interactor
    );

    scene.DragDropController.Initialize(
      scene.InputRender,
      scene.GameSetting.holdThreshold,
      scene.GameSetting.holdMoveTolerance
    );

    // =======================
    // Inventory
    // =======================

    var inventoryState = container.Get<InventoryState>();

    inventoryState.Initialize(
      scene.UIManager.OnEnterInventory,
      scene.UIManager.OnExitInventory
    );

    scene.HotbarController.Initialize(scene.InputRender, hotbarState);

    scene.InventoryController.Initialize(
      inventory,
      hotbarState,
      scene.HotbarInventoryView,
      scene.MainInventoryView,
      scene.DragGhost
    );

    // =======================
    // Grid
    // =======================

    scene.PlacementPreviewController.Initialze(scene.PreviewGridView);

    // =======================
    // TileMap
    // =======================

    scene.WorldTileManager.Initialize(
      scene.TilemapLayers,
      scene.GameSetting.TileLibrary,
      container.Get<GridConverter>(),
      cellResoulver
    );

    // =======================
    // Enemies
    // =======================

    scene.EnemySpawner.Initialze(spawnerHandle);

    // =======================
    // FlowState
    // =======================

    var gameplayState = container.Get<GameplayState>();

    gameplayState.Initialize(
      scene.PlayerController,
      scene.DragDropController
    );

    scene.TurnSystem.Initialize(
      energy,
      scene.CycleController);

    var interactionAction = new ItemInteractionAction(
      handleService,
      costResolver,
      excutor,
      scene.PlayerPivot,
      interactor,
      playerState,
      scene.DragDropController,
      particalService);
    
    RegisterObjects(scene.WorldTileManager, spawnerHandle);
  }
  
  private void RegisterObjects(
    WorldTileManager worldTileManager,
    SpawnerHandle spawnerHandle)
  {
    foreach (var ob in worldTileManager.Objects)
    {
      spawnerHandle.Register(ob);
    }
  }
}