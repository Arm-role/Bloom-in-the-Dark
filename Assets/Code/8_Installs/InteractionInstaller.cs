public class InteractionInstaller
{
    public void Install(DIContainerBase container, GameSceneInstaller scene)
    {
        var handleService = new InteractionHandleService();
        container.Register(handleService);

        var resolver = new InteractionTargetResolver(
            container.Get<GridConverter>(),
            scene.WorldTileManager
        );

        var dispatcher = new InteractionDispatcher(
            scene.GameSetting.DefaultLayerPriorityRule,
            resolver
        );

        container.Register(dispatcher);

        var strategyFactory = new ItemStrategyFactory(
            scene.GameSetting,
            container.Get<GridConverter>(),
            scene.PlacementPreviewController,
            scene.AreaCirclePreview,
            dispatcher,
            container.Get<TilemapService>(),
            scene.WorldTileManager,
            container.Get<GameObjectSpawner>(),
            container.Get<ParticalService>()
        );

        RegisterStrategies(handleService, strategyFactory);

        var executor = new WorldInteractionExecutor(
            scene.GameSetting.ItemsLibrary,
            container.Get<SpawnerHandle>(),
            container.Get<PlayerInventory>()
        );

        container.Register(executor);

        var gameplayState = container.Get<GameplayState>();

        gameplayState.Initialize(
            scene.PlayerController,
            scene.DragDropController
        );
    }

    private void RegisterStrategies(
        InteractionHandleService service,
        ItemStrategyFactory factory)
    {
        service.Register(EItemStrategyType.AreaLine, factory.CreateLineAttackStrategy());
        service.Register(EItemStrategyType.GridBased, factory.CreateGridBasedStategyBundle());
        service.Register(EItemStrategyType.AreaCircle, factory.CreateAreaCircleStategyBundle());
        service.Register(EItemStrategyType.GridTargeting, factory.CreateGridTargetingStategyBundle());
        service.Register(EItemStrategyType.DirectInteract, factory.CreateDirectInteractStategyBundle());
        service.Register(EItemStrategyType.ProximityCollider, factory.CreateProximityColliderStategyBundle());
    }
}
