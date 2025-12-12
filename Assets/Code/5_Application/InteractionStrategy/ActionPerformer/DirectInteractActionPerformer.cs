using System.Threading.Tasks;
using UnityEngine;

public class DirectInteractActionPerformer : IActionPerformer
{
    public void Setup() { }

    public async Task<bool> Execute(InteractionHandleContext context, IDataProvider data)
    {
        if (data is not DirectInteractData diData)
            return false;

        var interactable = diData.Target.GetInteractable();
        if (interactable == null)
            return false;

        return await interactable.TryInteract(context);
    }

    public bool CanExecute(InteractionHandleContext ctx, IDataProvider data)
    {
        if (data is not DirectInteractData diData)
            return false;

        if (!diData.Target.IsValid)
            return false;

        var interactable = diData.Target.GetInteractable();
        if (interactable == null)
            return false;

        return interactable.CanInteract(ctx);
    }
}