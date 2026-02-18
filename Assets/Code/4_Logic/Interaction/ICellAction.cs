using System.Threading.Tasks;

public interface ICellAction
{
  public InteractionStage Stage { get; }
  public ETargetType TargetType { get; }

  public Task<bool> CanProcess(
    InteractionIntent intent,
    IWorldCell cell);

  public Task<InteractionResult> Process(
    InteractionIntent intent,
    IWorldCell cell);
}