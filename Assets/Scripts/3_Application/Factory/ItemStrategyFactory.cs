public class ItemStrategyFactory
{
  private readonly WorldTileManager _worldTileManager;
  private readonly SpawnerHandle _spawner;

  private readonly SkillController _skillController;
  private readonly CellInteractionPipeline _pipeline;

  private readonly IPlacementPreview _placementPreview;
  private readonly IAreaCircleIndicatorPreview _areaCirclePreview;
  private readonly IConeIndicatorPreview _coneIndicatorPreview;

  public ItemStrategyFactory(
      WorldTileManager worldTileManager,
      SpawnerHandle spawner,
      IEnergyable energyable,
      CellInteractionPipeline pipeline,
      IPlacementPreview placementPreviewController,
      IAreaCircleIndicatorPreview areaCirclePreview,
      IConeIndicatorPreview conePreview)
  {
    //Service//
    _worldTileManager = worldTileManager;
    _spawner = spawner;
    _skillController = new SkillController(_spawner, energyable);
    _pipeline = pipeline;

    _placementPreview = placementPreviewController;
    //Preview/
    _placementPreview = placementPreviewController;
    _areaCirclePreview = areaCirclePreview;
    _coneIndicatorPreview = conePreview;
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

  public ItemStrategyBundle CreateSelfTargetStrategy()
  {
    var strategy = new SelfTargetStrategy(_worldTileManager);

    var validator = new SelfUseValidator();

    var action = new SelfUseActionPerformer(_skillController);
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

    var action = new SkillActionPerformer(_skillController);

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

  public ItemStrategyBundle CreateConeStrategy()
  {
    var shape = new ConeShape();

    var strategy = new ConeTargetStrategy(
        shape,
        _worldTileManager);

    var validator = new ConeValidator();

    var preview = new ConePreview(
        shape,
        _coneIndicatorPreview);

    var action = new SkillActionPerformer(_skillController);

    var targetingStrategy = new TargetingStrategy()
    {
      Strategy = strategy,
      Validator = validator,
      ConfigProvider = new ConeConfigProvider()
    };

    return new ItemStrategyBundle(
        targetingStrategy,
        preview,
        action);
  }

  public ItemStrategyBundle CreateAreaLineStrategy()
  {
    var shape = new AreaLineShape();

    var strategy = new AreaLineTargetStrategy(
        shape,
        _worldTileManager);

    var validator = new AreaLineValidator();

    var action = new SkillActionPerformer(_skillController);

    var targetingStrategy = new TargetingStrategy()
    {
      Strategy = strategy,
      Validator = validator,
      ConfigProvider = new AreaLineConfigProvider()
    };

    return new ItemStrategyBundle(
        targetingStrategy,
        null,
        action);
  }
}