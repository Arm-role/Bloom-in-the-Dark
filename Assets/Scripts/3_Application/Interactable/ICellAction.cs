using System.Threading.Tasks;

public interface ICellAction : IGameAction
{
  Task<bool> CanProcess(
   InteractionIntent intent,
   IWorldCell cell);

  Task<InteractionResult> Process(
   InteractionIntent intent,
   IWorldCell cell);
}
