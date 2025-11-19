using System.Threading.Tasks;
using UnityEngine;

public class DirectInteractActionPerformer : IActionPerformer
{
    public void Setup() { }

    public async Task<bool> Execute(InteractionHandleContext context, IDataProvider data)
    {
        if (data is not DirectInteractData directInteractData) return await Task.FromResult(false);
        if (directInteractData.PointerPosition.Value == null) return await Task.FromResult(false);

        if (directInteractData.Target.IsTile)
        {
            var tileState = directInteractData.Target.TileState;
            return await tileState.WorldInteractable.TryInteract(context);
        }

        return await Task.FromResult(false);
    }
}