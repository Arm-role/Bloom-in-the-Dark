using System.Threading.Tasks;

public interface ICellAction
{
    InteractionStage Stage { get; }

    Task<bool> CanProcess(
        InteractionIntent intent,
        IWorldCell cell);

    Task<InteractionResult> Process(
        InteractionIntent intent,
        IWorldCell cell);
}