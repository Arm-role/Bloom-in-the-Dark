using System.Threading.Tasks;
using UnityEngine;

public interface IActionPerformer
{
  bool CanExecute(
    InteractionIntent intent,
    TargetResult target);

  Task<InteractionExecutionPlan> Prepare(
    GameObject owner,
    InteractionIntent intent,
    TargetResult target);
}
