public class ItemStrategyFactory
{
    private GameSceneSettings _gameSceneSettings;

    private readonly WorldGridLogic _worldGridLogic;
    private readonly GridConverter _gridConverter;
    private readonly IPlacementPreview _placementPreview;
    private readonly AreaCirclePreview _areaCirclePreview;

    private readonly InteractionDispatcher _interactionDispatcher;
    private readonly WorldTileManager _worldTileManager;
    private readonly GameObjectSpawner _spawner;
    private readonly ParticalService _particalService;

    private readonly TilemapService _tilemapService;


    public ItemStrategyFactory(
        GameSceneSettings gameSetting,
        GridConverter gridConverter,
        IPlacementPreview placementPreviewController,
        AreaCirclePreview areaCirclePreview,
        InteractionDispatcher interactionDispatcher,
        TilemapService tilemapService,
        WorldTileManager worldTileManager,
        GameObjectSpawner spawner,
        ParticalService particalService)
    {
        //Data//
        _gameSceneSettings = gameSetting;

        //Preview/
        _placementPreview = placementPreviewController;
        _areaCirclePreview = areaCirclePreview;

        //Grid//
        _worldGridLogic = new WorldGridLogic();
        _gridConverter = gridConverter;

        //Service//
        _interactionDispatcher = interactionDispatcher;
        _tilemapService = tilemapService;
        _worldTileManager = worldTileManager;

        _spawner = spawner;
        _particalService = particalService;
    }
    public ItemStrategyBundle CreateGlobalStategyBundle()
    {
        float maxDistance = 2f;

        //Bundle//
        var pointerResolver = new GlobalPointerResolver(maxDistance);
        var detector = new GlobalDetector(pointerResolver, _interactionDispatcher);
        var validator = new GlobalValidator(_gameSceneSettings.InteractionRules);
        var action = new GlobalActionPerformer();
        var data = new GlobalData();

        return new ItemStrategyBundle(
          pointerResolver: pointerResolver,
          detector: detector,
          validator: validator,
          action: action,
          data: data);
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
             _worldTileManager);
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
        var pointerResolver = new GridTargetingPointerResolver(tileTargetLogic);
        var targetDetactor = new GridTargetingDetactor(pointerResolver, _interactionDispatcher);
        var validator = new GridTargetingValidator();
        var targetDetactorPreview = new GridTargetingPreview(_placementPreview, tileTargetLogic);
        var itemAction = new GridTargetingActionPerformer();
        var dataTransfer = new GridTargetingData();

        return new ItemStrategyBundle(
           detector: targetDetactor,
           targetDetectorPreview: targetDetactorPreview,
           validator: validator,
           action: itemAction,
           data: dataTransfer);
    }

    public ItemStrategyBundle CreateAreaCircleStategyBundle()
    {
        float xAngle = 55f;

        var skillInteractionController = new SkillInteractionController(_spawner);

        //Logic//
        var areCircleIndicator = new AreaCircleIndicator(xAngle);

        //Bundle//
        var pointerResolver = new AreaCirclePointerResolver(areCircleIndicator);
        var detector = new AreaCircleDetector(pointerResolver, _interactionDispatcher);
        var validator = new AreaCircleValidator();
        var skillIndicatorPreview = new AreaCircleSkillPreview(areCircleIndicator, _areaCirclePreview);
        var action = new AreaCircleActionPerformer(skillInteractionController);
        var data = new AreaCircleData();

        return new ItemStrategyBundle(
           pointerResolver: pointerResolver,
           detector: detector,
           validator: validator,
           skillIndicatorPreview: skillIndicatorPreview,
           action: action,
           data: data);
    }

    public ItemStrategyBundle CreateDirectInteractStategyBundle()
    {
        float maxDistance = 2f;

        //Bundle//
        var pointerResolver = new DirectInteractPointerResolver(maxDistance);
        var detector = new DirectInteractDetector(pointerResolver, _interactionDispatcher);
        var validator = new DirectInteractValidator();
        var action = new DirectInteractActionPerformer();
        var data = new DirectInteractData();

        return new ItemStrategyBundle(
          pointerResolver: pointerResolver,
          detector: detector,
          validator: validator,
          action: action,
          data: data);
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
