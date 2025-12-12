using System.Threading.Tasks;

public interface IActionPerformer
{
    void Setup();
    bool CanExecute(InteractionHandleContext ctx, IDataProvider data);
    Task<bool> Execute(InteractionHandleContext context, IDataProvider data);
}
