public class SceneBindingInstaller
{
  public void Install(DIContainerBase container, GameSceneInstaller scene)
  {
    var gameApplication = container.Get<GameApplication>();

    var spawnerHandle = container.Get<SpawnerHandle>();

    var state = container.Get<PlayerState>();
    var data = container.Get<PlayerData>();

    var playerCooldown = container.Get<CooldownContainer>();

    var hotbarState = container.Get<HotbarState>();
    var inventoryService = container.Get<InventoryService>();
    var inventory = container.Get<PlayerInventory>();
    var inventoryController = container.Get<InventoryController>();
    var inventoryScreen = container.Get<InventoryScreenController>();
    
    var health = container.Get<HealthResource>();
    var energy = container.Get<PlayerEnergy>();
    var actionLock = container.Get<PlayerActionLock>();
    var interactor = container.Get<PlayerInteractor>();
    
    var cellResoulver = container.Get<DefaultCellActionResolver>();
    var costResolver = container.Get<InteractionCostResolver>();
    var excutor = container.Get<WorldInteractionExecutor>();

    var handleService = container.Get<InteractionHandleService>();
    var playerState = container.Get<PlayerState>();
    var particalService = container.Get<ParticalService>();

    var strategyFactory = container.Get<ItemStrategyFactory>();

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
      inventoryScreen,
      actionLock,
      scene.PlayerController
    );

    scene.HotbarController.Initialize(scene.InputRender, hotbarState);

    scene.HotbarInventoryView.Initialize(scene.ItemIconDatabase);
    scene.MainInventoryView.Initialize(scene.ItemIconDatabase);

    scene.InventoryUI.Initialzed(
      scene.HotbarController,
      inventoryController
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
      actionLock,
      scene.PlayerController,
      scene.DragDropController
    );

    scene.TurnSystem.Initialize(
      energy,
      scene.CycleController,
      scene.TurnView);

    var interactionAction = new ItemInteractionAction(
      handleService,
      excutor,
      scene.PlayerPivot,
      interactor,
      playerState,
      scene.DragDropController,
      particalService,
      costResolver,
      playerCooldown);

    AddMockItem(scene, inventory);
    RegisterStrategies(
        handleService,
        strategyFactory);

    RegisterObjects(scene.WorldTileManager, spawnerHandle);

    inventoryController.Initialize();
  }

  private void AddMockItem(
      GameSceneInstaller scene,
      PlayerInventory inventory)
  {
    foreach (var item in scene.MockSettings.Items)
    {
      switch (item.Category)
      {
        case EItemCategory.Tool: inventory.AddItem(ItemFactory.Create(item), 1); break;
        case EItemCategory.Plant: inventory.AddItem(ItemFactory.Create(item), 10); break;
        case EItemCategory.Seed: inventory.AddItem(ItemFactory.Create(item), 10); break;
        case EItemCategory.Building: inventory.AddItem(ItemFactory.Create(item), 10); break;
      }
    }
  }

  private void RegisterStrategies(
      InteractionHandleService service,
      ItemStrategyFactory factory)
  {
    service.Register(EItemStrategyType.GridTargeting, factory.CreateGridTargetStrategy());
    service.Register(EItemStrategyType.DirectInteract, factory.CreateDirectInteractStrategy());
    service.Register(EItemStrategyType.Self, factory.CreateSelfTargetStrategy());

    service.Register(EItemStrategyType.AreaCircle, factory.CreateAreaCircleStrategy());
    service.Register(EItemStrategyType.AreaLine, factory.CreateAreaLineStrategy());

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