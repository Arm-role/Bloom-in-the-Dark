using System.Threading.Tasks;

public interface IWorldInteractable
{
    float InteractionPriority { get; }
    ETileBlockType Type { get; }

    Task<bool> TryInteract(InteractionHandleContext context);
    bool CanInteract(InteractionHandleContext context);
}
