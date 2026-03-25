public class SceneBindingInstaller
{
  public void Install(DIContainerBase container, GameSceneInstaller scene)
  {
    var gameApplication = container.Get<GameApplication>();

    var spawnerHandle = container.Get<SpawnerHandle>();

    var upgradeContainer = container.Get<GlobalUpgradeDomain>();
    var phaseStatService = container.Get<PhaseStatService>();
    var playerProgession = container.Get<PlayerProgression>();

    var itemFactory = container.Get<ItemFactory>();

    var state = container.Get<PlayerState>();

    var playerAnimationSystem = container.Get<CharacterAnimationSystem>();
    var playerAnimationTagService = container.Get<CharacterAnimationTagService>();

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

    var dragDropController = container.Get<DragDropController>();

    gameApplication.Initialize(
      scene.InputRender,
      scene.UpgradeManagerView);

    // =======================
    // Player
    // =======================

    scene.PlayerController.Initialize(
      scene.InputRender,
      scene.Scriptable.StatDatabase,
      state,
      phaseStatService,
      health,
      energy,
      interactor,
      playerAnimationSystem
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

    scene.HotbarInventoryView.Initialize(scene.Scriptable.ItemDatabase);
    scene.MainInventoryView.Initialize(scene.Scriptable.ItemDatabase);

    scene.InventoryUI.Initialzed(
      scene.HotbarController,
      inventoryController
      );

    // =======================
    // Upgrade
    // =======================
    scene.UpgradeRequestView.Initialize(scene.Scriptable.ItemDatabase);
    scene.UpgradeManagerView.Initialze(itemFactory, upgradeContainer, phaseStatService);
    scene.UsageLookup.Initialize(
      scene.Scriptable.RequestDatabase, 
      scene.UpgradeRequestView,
      scene.UpgradeManagerView
      );

    scene.ExpManagerView.Initialze(
      playerProgession,
      scene.UpgradeManagerView);

    // =======================
    // Grid
    // =======================

    scene.PlacementPreviewController.Initialze(scene.PreviewGridView);

    // =======================
    // TileMap
    // =======================

    scene.WorldTileManager.Initialize(
      scene.TilemapLayers,
      scene.Scriptable.TileLibrary,
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
      dragDropController
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
      dragDropController,
      costResolver,
      playerAnimationSystem,
      playerAnimationTagService,
      playerCooldown);

    AddMockItem(scene, itemFactory, inventory);
    RegisterStrategies(
        handleService,
        strategyFactory);

    RegisterObjects(scene.WorldTileManager, spawnerHandle);

    inventoryController.Initialize();

    dragDropController.RegisterHoverResolver(container.Get<WorldHoverResolver>());
    dragDropController.RegisterHoverResolver(container.Get<UIHoverResolver>());
  }

  private void AddMockItem(
      GameSceneInstaller scene,
      ItemFactory itemFactory,
      PlayerInventory inventory)
  {
    foreach (var item in scene.MockSettings.Items)
    {
      switch (item.Role)
      {
        case EItemRole.Tool: inventory.AddItem(itemFactory.Create(item), 1); break;
        default: inventory.AddItem(itemFactory.Create(item), 10); break;
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