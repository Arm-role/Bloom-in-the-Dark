using System.Threading.Tasks;

public interface IActionPerformer
{
  bool CanExecute(
      InteractionIntent intent,
      TargetResult target);

  Task<InteractionExecutionPlan> Prepare(
   InteractionIntent intent,
   TargetResult target);
}
