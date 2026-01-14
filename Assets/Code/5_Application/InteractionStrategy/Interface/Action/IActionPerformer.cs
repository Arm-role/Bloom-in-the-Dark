using System.Threading.Tasks;

public interface IActionPerformer
{
    bool CanExecute(
        InteractionHandleContext ctx,
        TargetResult target);

    Task<InteractionResult> Execute(
        InteractionHandleContext ctx,
        TargetResult target);
}
