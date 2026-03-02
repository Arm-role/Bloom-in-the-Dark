using System.Threading.Tasks;

public class TillGrassToSoilAction : ICellAction
{
  public InteractionStage Stage => InteractionStage.Main;
  private readonly IBaseTileData _baseTileData;
  public ETargetType TargetType { get; }

  public TillGrassToSoilAction(
    ETargetType targetType,
    IBaseTileData tileData)
  {
    TargetType = targetType;
    _baseTileData = tileData;
  }

  public Task<bool> CanProcess(
    InteractionIntent intent,
    IWorldCell cell)
  {
    if (cell.HasPlacedObject)
      return Task.FromResult(false);

    if (!intent.Is(EInteractionIntentType.Dig))
      return Task.FromResult(false);

    if (intent.SourceItem.Data is not ToolItem toolItem)
      return Task.FromResult(false);
    
    if (toolItem.ToolType != EToolType.Hoe)
      return Task.FromResult(false);

    if (cell is WorldCell wc && !wc.IsSingleLayer(ETileLayerType.Ground))
      return Task.FromResult(false);

    return Task.FromResult(true);
  }

  public Task<InteractionResult> Process(
    InteractionIntent intent,
    IWorldCell cell)
  {
    var result = new WorldAction
    {
      AddTile = _baseTileData,
      TileTargetLayer = ETileLayerType.Overlay
    };

    return Task.FromResult(InteractionResult.Consumed(cell, result, TargetType, ItemCooldownFeedback.None));
  }
}