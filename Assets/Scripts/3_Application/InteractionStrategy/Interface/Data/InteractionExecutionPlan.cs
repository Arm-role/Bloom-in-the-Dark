using System.Threading.Tasks;

public class InteractionExecutionPlan
{
  public InteractionIntent Intent;
  public ETargetType TargetMask;

  public System.Func<Task<InteractionResult>> Commit;
}