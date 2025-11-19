using System.Threading.Tasks;

public class GlobalActionPerformer : IActionPerformer
{
    public void Setup() { }

    public async Task<bool> Execute(InteractionHandleContext context, IDataProvider data)
    {
        if (data is not GlobalData globalData) return await Task.FromResult(false);

        if (globalData.Target.IsObject)
        {
            var interable = globalData.Target.WorldInteractable;
            return await interable.TryInteract(context);
        }

        return false;
    }
}