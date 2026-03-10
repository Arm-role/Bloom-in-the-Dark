using UnityEngine;

public class RuntimeInstaller
{
  public void Install(DIContainerBase container, GameSceneInstaller scene)
  {
    var pool = container.Get<IAdressablePoolService<GameObject>>();

    var spawner = new GameObjectSpawner(pool, scene.GameScriptableSetting.GameObjectLibrary);
    var spawnerHandle = new SpawnerHandle(spawner);
    var particle = new ParticalService(pool, scene.GameScriptableSetting.ParticleLibrary);

    // =======================
    // Tag
    // =======================

    TagLibrary.Initialize(scene.GameScriptableSetting.TagLibrary);

    // =======================
    // Player
    // =======================

    var state = new PlayerState(FacingDirection.Right);
    var data = new PlayerData(
        scene.GameScriptableSetting.MaxHP,
        scene.GameScriptableSetting.MaxHP,
        scene.GameScriptableSetting.MaxEnergy,
        scene.GameScriptableSetting.MaxEnergy,
        scene.GameScriptableSetting.MoveSpeed);

    // =======================
    // Animation
    // =======================

    var playerAnimationTagService = new CharacterAnimationTagService(scene.GameScriptableSetting.CharacterAnimationConfig);

    var playerAnimationSystem = new CharacterAnimationSystem(
      scene.GameScriptableSetting.AnimationLibrary);

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

    var hotbarState = new HotbarState(scene.GameScriptableSetting.HotbarSize);

    var hotbarLogic = new InventoryLogic(scene.GameScriptableSetting.HotbarSize);
    var mainInventoryLogic = new InventoryLogic(scene.GameScriptableSetting.InventorySize);

    var inventory = new PlayerInventory(
      ItemFactory.Create(scene.MockSettings.EmptyItem),
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
      scene.ItemIconDatabase
    );

    var inventoryScreenController = new InventoryScreenController(
      scene.InventoryUI,
      inventoryController
    );

    // =======================
    // Interactor
    // =======================

    var health = new HealthResource(data.MaxHealth);
    var playerEnergy = new PlayerEnergy(data.MaxEnergy);

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
        scene.GameScriptableSetting.TileLibrary
    );

    var factory = new CellInteractableFactory(ctx);

    var cellActionResolver = new DefaultCellActionResolver(
        factory);

    // =======================
    // Interaction
    // =======================

    var handleService = new InteractionHandleService();

    var executor = new WorldInteractionExecutor(
        spawnerHandle,
        scene.WorldTileManager,
        inventory);

    var pipeline = new CellInteractionPipeline();

    var strategyFactory = new ItemStrategyFactory(
        scene.WorldTileManager,
        spawnerHandle,
        interactor,
        pipeline,
        scene.PlacementPreviewController,
        scene.AreaCirclePreview
    );

    var initializer = new GameObjectInitialzer(
        scene.TurnSystem,
        spawnerHandle);

    var interactionRuntime = new InteractionRuntimeState();
    var costResolver = new InteractionCostResolver(
        scene.GameScriptableSetting.InteractionCostConfig,
        interactionRuntime
        );

    var worldHover = new WorldHoverResolver(
      scene.GameScriptableSetting.DetectionLayer
      );

    var uiHover = new UIHoverResolver(
      scene.GameMonoSetting.eventSystem
      );

    var dragDropController = new DragDropController(
        scene.InputRender,
        scene.GameScriptableSetting.holdThreshold,
        scene.GameScriptableSetting.holdMoveTolerance
        );

    container.Register(pool);

    container.Register(spawner);
    container.Register(spawnerHandle);
    container.Register(particle);

    container.Register(state);
    container.Register(data);

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