using System.Collections.Generic;

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

    scene.BaseBuildingController.Initialize(
      scene.Scriptable.StatDatabase,
      phaseStatService,
      new BuildingHealth(1000)
    );

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
    scene.UpgradeRequestView.Initialize(scene.Scriptable.ItemDatabase);
    scene.UpgradeManagerView.Initialze(itemFactory, upgradeContainer, phaseStatService);
    scene.AltarController.Initialize(
      scene.Scriptable.RequestDatabase,
      scene.UpgradeRequestView,
      scene.UpgradeManagerView,
      scene.ProgressionView,
      playerProgession
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

    scene.WorldTileManager.Initialize(
      scene.TilemapLayers,
      scene.Scriptable.TileLibrary,
      container.Get<GridConverter>(),
      cellResoulver,
      container.Get<WorldZoneManager>()
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
      playerCooldown);

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

    initializer.ManualSubscribe(scene.PlayerController.gameObject);
    initializer.ManualSubscribe(scene.BaseBuildingController.gameObject);

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