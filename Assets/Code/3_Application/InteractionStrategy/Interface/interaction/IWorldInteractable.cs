using System.Threading.Tasks;

public interface IWorldInteractable
{
    ETileCapability Type { get; }

    Task<bool> CanInteract(InteractionIntent intent);
    Task<bool> TryInteract(InteractionIntent intent);
}