using System.Threading.Tasks;

public interface IActionPerformer
{
    bool CanExecute(
        InteractionIntent intent,
        TargetResult target);

    Task<InteractionResult> Execute(
        InteractionIntent intent,
        TargetResult target);
}
