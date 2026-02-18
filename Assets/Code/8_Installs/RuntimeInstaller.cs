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
        // Inventory
        // =======================

        var hotbarState = new HotbarState(scene.GameSetting.HotbarSize);

        var inventory = new PlayerInventory(
            hotbarState,
            scene.GameSetting.HotbarSize,
            scene.GameSetting.InventorySize
        );

        // =======================
        // Interactor
        // =======================
        
        var health = new HealthResource(data.MaxHealth);
        var playerEnergy = new PlayerEnergy(data.MaxEnergy);

        var interactor = new PlayerInteractor(playerEnergy, health, inventory);
        
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
        
        AddMockItem(scene, inventory);
        RegisterStrategies(
            handleService,
            strategyFactory);

        container.Register(pool);

        container.Register(spawner);
        container.Register(spawnerHandle);
        container.Register(particle);

        container.Register(state);
        container.Register(data);

        container.Register(hotbarState);
        container.Register(inventory);
        
        container.Register(health);
        container.Register(playerEnergy);
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

    private void AddMockItem(
        GameSceneInstaller scene,
        PlayerInventory inventory)
    {
        foreach (var item in scene.MockSettings.items)
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
}