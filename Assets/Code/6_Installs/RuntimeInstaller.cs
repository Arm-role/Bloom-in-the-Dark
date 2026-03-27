using UnityEngine;

public class RuntimeInstaller
{
  public void Install(DIContainerBase container, GameSceneInstaller scene)
  {
    var pool = container.Get<IAdressablePoolService<GameObject>>();

    var spawner = new GameObjectSpawner(pool, scene.Scriptable.GameObjectLibrary);
    var spawnerHandle = new SpawnerHandle(spawner);
    var particle = new ParticalService(pool, scene.Scriptable.ParticleLibrary);

    // =======================
    // Tag
    // =======================

    TagLibrary.Initialize(scene.Scriptable.TagLibrary);

    // =======================
    // Progression
    // =======================


    var upgradeContainer = new GlobalUpgradeDomain();

    var phaseStatService = new PhaseStatService(
      scene.Scriptable.PhaseStatConfig,
      scene.Scriptable.StatDatabase,
      upgradeContainer
      );

    var playerProgession = new PlayerProgression(
      scene.Scriptable.PhaseStatConfig.LevelStart);

    // =======================
    // Item
    // =======================

    var itemFactory = new ItemFactory(
      scene.Scriptable.ItemDatabase,
      scene.Scriptable.StatDatabase,
      upgradeContainer
      );

    // =======================
    // Player
    // =======================

    var state = new PlayerState(FacingDirection.Right);

    // =======================
    // Animation
    // =======================

    var playerAnimationTagService = new CharacterAnimationTagService(scene.Scriptable.CharacterAnimationConfig);

    var playerAnimationSystem = new CharacterAnimationSystem(
      scene.Scriptable.AnimationLibrary);

    // =======================
    // Cooldown
    // =======================

    var timeSource = new UnityTimeSource();
    var actionLock = new PlayerActionLock(timeSource);
    var cooldownService = new CooldownService(timeSource);

    var playerCooldown = cooldownService.GetOrCreate(scene.PlayerController);

    // =======================
    // Inventory
    // =======================

    var hotbarState = new HotbarState(scene.Scriptable.HotbarSize);

    var hotbarLogic = new InventoryLogic(scene.Scriptable.HotbarSize);
    var mainInventoryLogic = new InventoryLogic(scene.Scriptable.InventorySize);

    var inventory = new PlayerInventory(
      itemFactory.Create(scene.MockSettings.EmptyItem),
      hotbarState,
      hotbarLogic,
      mainInventoryLogic
    );

    var inventoryService = new InventoryService(inventory);

    var inventoryController = new InventoryController(
      scene.HotbarInventoryView,
      scene.MainInventoryView,
      hotbarState,
      inventoryService,
      scene.DragGhost,
      playerCooldown,
      scene.Scriptable.ItemDatabase
    );

    var inventoryScreenController = new InventoryScreenController(
      scene.InventoryUI,
      inventoryController
    );

    // =======================
    // Interactor
    // =======================

    var health = new HealthResource(100);
    var playerEnergy = new PlayerEnergy(
      scene.Scriptable.StatDatabase,
      phaseStatService,
      scene.Scriptable.PhaseStatConfig.Key);

    var interactor = new PlayerInteractor(
      playerEnergy,
      health,
      inventory,
      actionLock,
      playerCooldown);

    // =======================
    // Grid
    // =======================

    var gridConverter = new GridConverter(scene.MainTilemap);

    // =======================
    // TileMap
    // =======================

    var ctx = new CellActionContext
    (
        scene.Scriptable.TileLibrary
    );

    var factory = new GameActionFactory(ctx);

    var cellActionResolver = new DefaultCellActionResolver(
        factory);

    // =======================
    // Interaction
    // =======================

    var handleService = new InteractionHandleService();

    var executor = new WorldInteractionExecutor(
        spawnerHandle,
        playerProgession,
        itemFactory,
        scene.WorldTileManager,
        inventory);

    var cellPipeline = new CellInteractionPipeline();

    var strategyFactory = new ItemStrategyFactory(
        scene.WorldTileManager,
        spawnerHandle,
        interactor,
        cellPipeline,
        scene.PlacementPreviewController,
        scene.AreaCirclePreview
    );

    var initializer = new GameObjectInitialzer(
        scene.TurnSystem,
        spawnerHandle,
        executor);

    var interactionRuntime = new InteractionRuntimeState();
    var costResolver = new InteractionCostResolver(
        scene.Scriptable.InteractionCostConfig,
        interactionRuntime
        );

    var worldHover = new WorldHoverResolver(
      scene.Scriptable.DetectionLayer
      );

    var uiHover = new UIHoverResolver(
      scene.GameMonoSetting.eventSystem
      );

    var dragDropController = new DragDropController(
        scene.InputRender,
        scene.Scriptable.holdThreshold,
        scene.Scriptable.holdMoveTolerance
        );

    container.Register(pool);

    container.Register(spawner);
    container.Register(spawnerHandle);
    container.Register(particle);

    container.Register(upgradeContainer);
    container.Register(phaseStatService);
    container.Register(playerProgession);

    container.Register(itemFactory);

    container.Register(state);

    container.Register(playerAnimationTagService);
    container.Register(playerAnimationSystem);

    container.Register(playerCooldown);

    container.Register(hotbarState);
    container.Register(inventoryService);
    container.Register(inventory);
    container.Register(inventoryController);
    container.Register(inventoryScreenController);

    container.Register(health);
    container.Register(playerEnergy);
    container.Register(actionLock);
    container.Register(interactor);

    container.Register(gridConverter);

    container.Register(factory);
    container.Register(cellActionResolver);

    container.Register(handleService);
    container.Register(strategyFactory);
    container.Register(executor);

    container.Register(initializer);

    container.Register(costResolver);

    container.Register(worldHover);
    container.Register(uiHover);
    container.Register(dragDropController);
  }
}