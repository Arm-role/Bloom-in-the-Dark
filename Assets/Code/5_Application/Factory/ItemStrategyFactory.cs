using UnityEngine;

public class ItemStrategyFactory
{
    private GameSceneSettings _gameSceneSettings;

    private readonly WorldTileManager _worldTileManager;
    private readonly SpawnerHandle _spawner;

    private readonly SkillSpawnController _skillSpawnController;
    private readonly CellInteractionPipeline _pipeline;

    private readonly IPlacementPreview _placementPreview;
    private readonly IAreaCircleIndicatorPreview _areaCirclePreview;

    public ItemStrategyFactory(
        GameSceneSettings gameSetting,
        WorldTileManager worldTileManager,
        SpawnerHandle spawner,
        CellInteractionPipeline pipeline,
        IPlacementPreview placementPreviewController,
        IAreaCircleIndicatorPreview areaCirclePreview)
    {
        //Data//
        _gameSceneSettings = gameSetting;

        //Service//
        _worldTileManager = worldTileManager;
        _spawner = spawner;
        _skillSpawnController = new SkillSpawnController(_spawner);
        _pipeline = pipeline;

        _placementPreview = placementPreviewController;
        //Preview/
        _placementPreview = placementPreviewController;
        _areaCirclePreview = areaCirclePreview;
    }

    public ItemStrategyBundle CreateGridTargetStrategy()
    {
        var strategy = new GridTargetStrategy(
            _worldTileManager);

        var validator = new GridTargetValidator();

        var preview = new GridTargetingPreview(
            _placementPreview);

        var action = new CellActionPerformer(_pipeline);
        var targetingStrategy = new TargetingStrategy()
        {
            Strategy = strategy,
            Validator = validator,
            ConfigProvider = new GridTargetConfigProvider()
        };

        return new ItemStrategyBundle(
            targetingStrategy,
            preview,
            action);
    }

    public ItemStrategyBundle CreateDirectInteractStrategy()
    {
        var strategy = new DirectInteractStrategy(
            _worldTileManager);

        var validator = new DirectInteractValidator();

        var action = new CellActionPerformer(_pipeline);
        var targetingStrategy = new TargetingStrategy()
        {
            Strategy = strategy,
            Validator = validator,
            ConfigProvider = new DiractionConfigProvider()
        };

        return new ItemStrategyBundle(
            targetingStrategy,
            null,
            action);
    }

    public ItemStrategyBundle CreateAreaCircleStrategy()
    {
        var shape = new AreaCircleShape();

        var strategy = new AreaCircleTargetStrategy(
            shape,
            _worldTileManager);

        var validator = new AreaCircleValidator();

        var preview = new AreaCirclePreview(
            shape,
            _areaCirclePreview);

        var action = new AreaCircleActionPerformer(_skillSpawnController);
        var targetingStrategy = new TargetingStrategy()
        {
            Strategy = strategy,
            Validator = validator,
            ConfigProvider = new AreaCircleConfigProvider()
        };

        return new ItemStrategyBundle(
            targetingStrategy,
            preview,
            action);
    }

    public ItemStrategyBundle CreateSelfTargetStrategy()
    {
        var strategy = new SelfTargetStrategy(_worldTileManager);

        var validator = new SelfUseValidator();

        var action = new SelfUseActionPerformer(_skillSpawnController);
        var targetingStrategy = new TargetingStrategy()
        {
            Strategy = strategy,
            Validator = validator,
            ConfigProvider = new SelfConfigProvider()
        };

        return new ItemStrategyBundle(
            targetingStrategy,
            preview: null,
            action);
    }
}