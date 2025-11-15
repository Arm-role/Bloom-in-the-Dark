using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class WorldInteractable : MonoBehaviour, IWorldInteractable
{
    [SerializeField] private WorldInteractableStrategy _strategy;

    public EWorldInteractableType Type => _strategy.Type;
    public WorldInteractableStrategy Strategy => _strategy;
    public float InteractionPriority => _strategy != null ? _strategy.Priority : 0f;

    public bool CanInteract(InteractionHandleContext ctx)
    {
        if (_strategy == null) return false;
        return _strategy.CanInteract(ctx);
    }

    public Task<bool> TryInteract(InteractionHandleContext ctx)
    {
        if (_strategy == null) return Task.FromResult(false);
        return _strategy.TryInteract(ctx);
    }
}