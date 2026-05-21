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

    var health = container.Get<PlayerHealth>();
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

    var initializer = container.Get<GameObjectInitializer>();

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

    // BaseBuildingController setup เลือดผ่าน config ใน Awake — ไม่ต้อง DI Initialize

    // =======================
    // Inventory
    // =======================

    scene.HotbarController.Initialize(scene.InputRender, hotbarState);

    scene.HotbarInventoryView.Initialize(scene.Scriptable.ItemDatabase);
    scene.MainInventoryView.Initialize(scene.Scriptable.ItemDatabase);

    scene.InventoryUI.Initialized(
      scene.HotbarController,
      inventoryController
      );

    // =======================
    // Upgrade
    // =======================
    scene.UpgradeManagerView.Initialze(itemFactory, upgradeContainer, phaseStatService, scene.Scriptable.ItemDatabase);
    scene.AltarSuggestionView.Initialize(scene.Scriptable.ItemDatabase);
    scene.AltarController.Initialize(
      scene.Scriptable.ItemDatabase,
      scene.Scriptable.AltarRecipeDatabase,
      scene.UpgradeManagerView,
      scene.ProgressionView,
      scene.AltarSuggestionView,
      playerProgession,
      itemFactory,
      inventory,
      scene.EnemySpawner,
      upgradeContainer
      );

    scene.ExpManagerView.Initialze(
      playerProgession,
      scene.UpgradeManagerView,
      scene.UpgradeManagerView);

    // =======================
    // Grid
    // =======================

    scene.PlacementPreviewController.Initialze(scene.PreviewGridView);

    // =======================
    // TileMap
    // =======================

    var zoneManager = container.Get<WorldZoneManager>();

    scene.WorldTileManager.Initialize(
      scene.TilemapLayers,
      scene.Scriptable.TileLibrary,
      container.Get<GridConverter>(),
      cellResoulver,
      zoneManager
    );

    // =======================
    // Enemies
    // =======================

    scene.EnemySpawner.Initialze(spawnerHandle);

    // =======================
    // FlowState
    // =======================

    scene.TurnSystem.Initialize(
      scene.Scriptable.StatDatabase,
      phaseStatService,
      scene.Scriptable.PhaseStatConfig.Key,
      scene.PlayerController,
      scene.PlayerController,
      interactor,
      scene.CycleController,
      scene.TurnView);

    var interactionAction = new ItemInteractionAction(
      handleService,
      excutor,
      scene.PlayerTransform,
      interactor,
      playerState,
      dragDropController,
      costResolver,
      playerAnimationSystem,
      playerAnimationTagService,
      playerCooldown,
      scene.Scriptable.GlobalInteractionConfig);

    // =======================
    // AddModules
    // =======================

    var upgradeState = container.Get<UpgradeState>();
    var gameplayState = container.Get<GamePlayState>();
    var inventoryState = container.Get<InventoryState>();

    var stateMachine = container.Get<GameStateMachine>();

    var inventoryCooldownController = container.Get<InventoryCooldownController>();

    inventoryState.AddSystem(inventoryScreen);
    inventoryState.AddSystem(inventoryCooldownController);
    inventoryState.AddSystem(actionLock);

    gameplayState.AddSystem(inventoryCooldownController);
    gameplayState.AddSystem(actionLock);
    gameplayState.AddSystem(dragDropController);

    stateMachine.AddStateListener(scene.PlayerController);

    scene.PauseMenuController.Initialize(stateMachine, scene.InputRender);

    initializer.ManualSubscribe(scene.PlayerController.gameObject);
    initializer.ManualSubscribe(scene.BaseBuildingController.gameObject);

    AddMockItem(scene, itemFactory, inventory);
    RegisterStrategies(
        handleService,
        strategyFactory);

    strategyFactory.SetSkillChannelContext(scene.InputRender, interactor);

    RegisterObjects(scene.WorldTileManager, spawnerHandle);

    zoneManager.ZoneChange(
      phaseStatService.GetStat(scene.Scriptable.StatDatabase.FarmArea),
      scene.Scriptable.ZoneUpgradeConfig.ZoneFlags);

    inventoryController.Initialize();

    dragDropController.RegisterHoverResolver(container.Get<WorldHoverResolver>());
    dragDropController.RegisterHoverResolver(container.Get<UIHoverResolver>());
  }

  private void AddMockItem(
      GameSceneInstaller scene,
      ItemFactory itemFactory,
      PlayerInventory inventory)
  {
    var single = TagLibrary.Get("Item.SingleStack");
    foreach (var item in scene.MockSettings.Items)
    {
      int amount = item.HasTag(single) ? 1 : 10;
      inventory.AddItem(itemFactory.Create(item), amount);
    }
  }

  private void RegisterStrategies(
      InteractionHandleService service,
      ItemStrategyFactory factory)
  {
    service.Register(EItemStrategyType.GridBased, factory.CreateGridBasedStrategy());
    service.Register(EItemStrategyType.GridTargeting, factory.CreateGridTargetStrategy());
    service.Register(EItemStrategyType.DirectInteract, factory.CreateDirectInteractStrategy());
    service.Register(EItemStrategyType.Self, factory.CreateSelfTargetStrategy());

    service.Register(EItemStrategyType.AreaCircle, factory.CreateAreaCircleStrategy());
    service.Register(EItemStrategyType.AreaCone, factory.CreateConeStrategy());
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