using Codice.Client.Common;
using NUnit.Framework.Constraints;
using UnityEngine.Tilemaps;

public class ItemStrategyFactory
{
    private GameSceneSettings _gameSceneSettings;

    private readonly WorldGridLogic _worldGridLogic;
    private readonly GridConverter _gridConverter;
    private readonly IPlacementPreview _placementPreview;
    private readonly AreaCirclePreview _areaCirclePreview;

    private readonly InteractionTargetResolver _interactionTarget;
    private readonly WorldTileManager _worldTileManager;
    private readonly GameObjectSpawner _spawner;
    private readonly ParticalService _particalService;

    private readonly TilemapService _groundService;
    private readonly TilemapService _overlayService;


    public ItemStrategyFactory(
        GameSceneSettings gameSetting,
        GridConverter gridConverter,
        IPlacementPreview placementPreviewController,
        AreaCirclePreview areaCirclePreview,
        InteractionTargetResolver interactionTarget,
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
        _interactionTarget = interactionTarget;
        _worldTileManager = worldTileManager;


        _spawner = spawner;
        _particalService = particalService;

        _worldTileManager.Initialize();

        if (_worldTileManager.TryGetTilemap(ETileLayerType.Ground, out var groundTilemap))
        {
            _groundService = new TilemapService(groundTilemap, gridConverter, worldTileManager, ETileLayerType.Ground);
        }

        if (_worldTileManager.TryGetTilemap(ETileLayerType.Overlay, out var overlayTilemap))
        {
            _overlayService = new TilemapService(overlayTilemap, gridConverter, worldTileManager, ETileLayerType.Overlay);
        }
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
        var targetDetactor = new GridTargetingDetactor(tileTargetLogic);
        var targetDetactorPreview = new GridTargetingPreview(_placementPreview, tileTargetLogic);

        var itemAction = new GridTargetingActionPerformer(
            _gameSceneSettings.TileLibrary,
            _overlayService
            );

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
        var cropPlacement = new CropPlacementSystem(_overlayService, _spawner);

        return new ItemStrategyBundle(
           detector: new DirectInteractDetector(_interactionTarget, maxDistance),
           validator: new DirectInteractValidator(),
           action: new DirectInteractActionPerformer(cropPlacement),
           data: new DirectInteractData());
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
