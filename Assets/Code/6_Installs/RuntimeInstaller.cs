using UnityEngine;

public class RuntimeInstaller
{
  public void Install(DIContainerBase container, GameSceneInstaller scene)
  {
    var pool = container.Get<IAdressablePoolService<GameObject>>();

    var spawner = new GameObjectSpawner(pool, scene.GameSetting.GameObjectLibrary);
    var spawnerHandle = new SpawnerHandle(spawner);
    var particle = new ParticalService(pool, scene.GameSetting.ParticleLibrary);

    // =======================
    // Player
    // =======================

    var state = new PlayerState(FacingDirection.Right);
    var data = new PlayerData(
        scene.GameSetting.MaxHP,
        scene.GameSetting.MaxHP,
        scene.GameSetting.MaxEnergy,
        scene.GameSetting.MaxEnergy,
        scene.GameSetting.MoveSpeed);

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

    var hotbarState = new HotbarState(scene.GameSetting.HotbarSize);

    var hotbarLogic = new InventoryLogic(scene.GameSetting.HotbarSize);
    var mainInventoryLogic = new InventoryLogic(scene.GameSetting.InventorySize);

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
      playerCooldown
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
        scene.GameSetting.TileLibrary
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
        scene.GameSetting,
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
        scene.GameSetting.InteractionCostConfig,
        interactionRuntime);

    container.Register(pool);

    container.Register(spawner);
    container.Register(spawnerHandle);
    container.Register(particle);

    container.Register(state);
    container.Register(data);

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
  }
}