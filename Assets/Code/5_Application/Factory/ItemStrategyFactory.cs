using UnityEngine.Tilemaps;

public class ItemStrategyFactory
{
    private GameSceneSettings _gameSceneSettings;

    private readonly Tilemap _tilemap;
    private readonly TilemapService _tilemapService;

    private readonly WorldGridLogic _worldGridLogic;
    private readonly GridConverter _gridConverter;
    private readonly IPlacementPreview _placementPreview;
    private readonly AreaCirclePreview _areaCirclePreview;

    private readonly InteractionTargetResolver _interactionTarget;
    private readonly GameObjectSpawner _spawner;
    private readonly ParticalService _particalService;

    public ItemStrategyFactory(
        GameSceneSettings gameSetting,
        Tilemap mainTilemap,
        GridConverter gridConverter,
        WorldTileState worldTileState,
        IPlacementPreview placementPreviewController,
        AreaCirclePreview areaCirclePreview,
        InteractionTargetResolver interactionTarget,
        GameObjectSpawner spawner,
        ParticalService particalService)
    {
        //Data//
        _gameSceneSettings = gameSetting;
        _tilemap = mainTilemap;

        //Preview/
        _placementPreview = placementPreviewController;
        _areaCirclePreview = areaCirclePreview;

        //Grid//
        _worldGridLogic = new WorldGridLogic();
        _gridConverter = gridConverter;

        //Service//
        _interactionTarget = interactionTarget;
        _spawner = spawner;
        _particalService = particalService;

        _tilemapService = new TilemapService(_tilemap, gridConverter, worldTileState);
    }

    public ItemStrategyBundle CreateGridBasedStategyBundle()
    {
        //Logic//
        var placementController = new PlacementController(_gridConverter, _worldGridLogic);

        //Bundle//
        var targetDetactor = new GridBaseDetactor(placementController);
        var targetDetactorPreview = new GridBasePreview(_placementPreview, placementController);

        var itemAction = new GridBaseActionPerformer(
            _gameSceneSettings.TileLibrary,
             _tilemapService);
        var dataTransfer = new GridBaseData();

        return new ItemStrategyBundle(
           detector: targetDetactor,
           targetDetectorPreview: targetDetactorPreview,
           action: itemAction,
           data: dataTransfer);
    }
    public ItemStrategyBundle CreateGridTargetingStategyBundle()
    {
        //Logic//
        var tileTargetLogic = new TileTargetLogic(_gridConverter);

        //Bundle//
        var targetDetactor = new GridTargetingDetactor(tileTargetLogic);
        var targetDetactorPreview = new GridTargetingPreview(_placementPreview, tileTargetLogic);

        var itemAction = new GridTargetingActionPerformer(
            _gameSceneSettings.TileLibrary,
            _tilemapService);

        var dataTransfer = new GridTargetingData();

        return new ItemStrategyBundle(
           detector: targetDetactor,
           targetDetectorPreview: targetDetactorPreview,
           action: itemAction,
           data: dataTransfer);
    }

    public ItemStrategyBundle CreateAreaCircleStategyBundle()
    {
        float _xAngle = 55f;
        float _rangeRadius = 10f;
        float _healRadius = 1f;

        var skillInteractionController = new SkillInteractionController(_particalService);

        //Logic//
        var areCircleIndicator = new AreaCircleIndicator(_xAngle, _rangeRadius, _healRadius);

        //Bundle//
        var targetDetactor = new AreaCircleDetector(areCircleIndicator);
        var skillPreview = new AreaCircleSkillPreview(areCircleIndicator, _areaCirclePreview);
        var itemAction = new AreaCircleActionPerformer(skillInteractionController);
        var dataTransfer = new AreaCircleData();

        return new ItemStrategyBundle(
           detector: targetDetactor,
           skillIndicatorPreview: skillPreview,
           action: itemAction,
           data: dataTransfer);
    }

    public ItemStrategyBundle CreateDirectInteractStategyBundle()
    {
        float maxDistance = 5f;
        var cropPlacement = new CropPlacementSystem(_tilemapService, _spawner);

        //Bundle//
        var targetDetactor = new DirectInteractDetector(_interactionTarget, maxDistance);
        var itemAction = new DirectInteractActionPerformer(cropPlacement);
        var dataTransfer = new DirectInteractData();

        return new ItemStrategyBundle(
           detector: targetDetactor,
           action: itemAction,
           data: dataTransfer);
    }

    public ItemStrategyBundle CreateProximityColliderStategyBundle()
    {
        //Logic//
        var proximityDetector = new ProximityDetector();

        //Bundle//
        var targetDetactor = new ProximityInteractionHandler(proximityDetector);
        var itemAction = new ProximityActionPerformer();
        var dataTransfer = new ProximityInteractionData();

        return new ItemStrategyBundle(
           detector: targetDetactor,
           action: itemAction,
           data: dataTransfer);
    }

    public ItemStrategyBundle CreateAreaConeStategyBundle()
    {
        return null;
    }
    public ItemStrategyBundle CreateAreaLineStategyBundle()
    {
        return null;
    }
}
