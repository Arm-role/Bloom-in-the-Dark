using System.Threading.Tasks;

public interface IActionPerformer
{
    void Setup();
    Task<bool> Execute(InteractionHandleContext context, IDataProvider data);
}